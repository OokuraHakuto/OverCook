using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;
// 完成版（ビルド時）でのみInput Systemを読み込む
#if !UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class PlayerController : MonoBehaviour
{
    // --- 両方のモードで共通して使う変数 ---
    [Header("移動速度")]
    public float moveSpeed = 10f; // キャラクターの移動速度
    [Header("回転の速さ")]
    public float rotateSpeed = 10f; // キャラクターの回転の追従速度

    [Header("インタラクト設定")]
    public float interactDistance = 3f; // 目の前を調べる距離
    public LayerMask interactableLayer; // レーザービームが当たるレイヤー

    [Header("インタラクトの判定幅（左右）")]
    [Tooltip("0 = まっすぐ, 0.5 = 左右に広がる")]
    [Range(0f, 1f)]
    public float interactWidth = 0.3f; // 左右の広がり

    [Header("インタラクトの判定幅（上下）")]
    [Tooltip("0 = まっすぐ, 0.5 = 上下に広がる")]
    [Range(0f, 1f)]
    public float verticalAngle = 0.3f; // 上下の角度

    [Header("アニメーション連動設定")]
    public Transform idleAnchor; // 普段の親（IdleAnchor）
    public Transform mixAnchor;  // 混ぜる時の親（MixAnchor）

    private Coroutine mixMotionCoroutine; // 連打制御用

    private Rigidbody rb; // 物理演算を管理するRigidbodyコンポーネント
    private Vector2 moveInput;         // 最終的な移動量
    private Vector2 gamepadInput;      // パッドからの入力値
    private Vector2 keyboardInput;     // キーボードからの入力値

    private Animator anim;

    [Header("アイテム保持設定")]
    public Transform handPosition; // アイテムを持つ位置（インスペクタで設定）
    public GameObject heldItem;    // 今持っているアイテム（内部用）

    [Header("プレイヤー番号")]
    public int playerID = 1; // プレイヤー番号（1か2）をインスペクタで指定

    [Header("投げる設定")]
    public float throwForceForward = 10f; // 前に飛ばす力
    public float throwForceUp = 5f;       // 上に浮かせる力

    // パッド入力
    public void OnMove(InputAction.CallbackContext context)
    {
        // パッドやキーボードからの入力をVector2として受け取る
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // ボタンが押された瞬間
        if (context.performed)
        {
            DoInteract(); // 共通のインタラクト関数を呼ぶ
        }
    }

    // ゲーム開始時に一度だけ呼ばれ、自分自身のコンポーネントを取得する
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        anim = GetComponent<Animator>();
    }

    // 毎フレーム、キーボード入力を直接チェックする
    void Update()
    {
        // キーボード入力をリセット
        keyboardInput = Vector2.zero;

        // キーボード入力のチェック
        if (playerID == 1)
        {
            if (Input.GetKey(KeyCode.W)) { keyboardInput.y = 1; }
            if (Input.GetKey(KeyCode.S)) { keyboardInput.y = -1; }
            if (Input.GetKey(KeyCode.A)) { keyboardInput.x = -1; }
            if (Input.GetKey(KeyCode.D)) { keyboardInput.x = 1; }

            // インタラクト (Eキー)
            if (Input.GetKeyDown(KeyCode.E))
            {
                DoInteract();
            }

            // 投げる
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ThrowItem();
            }
        }
        else if (playerID == 2)
        {
            if (Input.GetKey(KeyCode.UpArrow)) { keyboardInput.y = 1; }
            if (Input.GetKey(KeyCode.DownArrow)) { keyboardInput.y = -1; }
            if (Input.GetKey(KeyCode.LeftArrow)) { keyboardInput.x = -1; }
            if (Input.GetKey(KeyCode.RightArrow)) { keyboardInput.x = 1; }

            // インタラクト (Pキー)
            if (Input.GetKeyDown(KeyCode.P))
            {
                DoInteract();
            }

            //投げる
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ThrowItem();
            }
        }

        // パッド入力とキーボード入力を合体させる
        moveInput = gamepadInput + keyboardInput;

        // 正規化
        if (moveInput.magnitude > 1f)
        {
            moveInput.Normalize();
        }
    }


    // 物理演算のタイミングで一定間隔で呼ばれる
    void FixedUpdate()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isPlaying)
        {
            rb.velocity = Vector3.zero; // 滑らないようにピタッと止める
            if (anim != null) anim.SetFloat("Speed", 0f); // 走りモーションも止める
            return; // ここで処理終了（下の移動処理には行かせない）
        }

        // 2Dの入力を3D空間の移動ベクトルに変換する
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        // Rigidbodyの速度を更新してキャラクターを物理的に動かす（正規化して斜め移動対策）
        rb.velocity = movement.normalized * moveSpeed;

        // キャラクターの向きを入力方向に合わせる（入力がある時だけ）
        if (anim != null)
        {
            anim.SetFloat("Speed", movement.magnitude);
        }

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }


    // 物を投げる処理
    public void ThrowItem()
    {
        // 何も持ってなかったら投げられない
        if (heldItem == null) return;

        // ボウル側の「持たれているフラグ」を解除する
        Bowl bowl = heldItem.GetComponent<Bowl>();
        if (bowl != null)
        {
            bowl.OnReleased();
        }

        // 親子関係を解除（手から離す）
        heldItem.transform.SetParent(null);

        // 物理演算と当たり判定を復活させる
        Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
        Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
        Collider playerCol = GetComponent<Collider>();

        // コライダー復活
        foreach (Collider c in cols)
        {
            c.enabled = true;

            if (playerCol != null)
            {
                Physics.IgnoreCollision(c, playerCol, true);
            }
        }

        // もし親にコライダーがあればそれも復活
        Collider parentCol = heldItem.GetComponent<Collider>();
        if (parentCol != null) parentCol.enabled = true;

        // 物理演算ON
        if (itemRb != null)
        {
            itemRb.isKinematic = false; // 物理に従うようにする
            itemRb.useGravity = true;   // 重力をON

            // 力を加える（プレイヤーの向いている方向 + 上方向）
            Vector3 throwDir = transform.forward * throwForceForward + Vector3.up * throwForceUp;
            itemRb.AddForce(throwDir, ForceMode.Impulse); // 瞬間的な力を加える
        }

        // アニメーション等の処理
        if (anim != null)
        {
            anim.SetBool("IsHolding", false);
            anim.SetTrigger("Put"); // 置くモーションを流用
        }

        // 投げる音
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.seThrow);
        }

        // 手持ち情報を空にする
        heldItem = null;
    }


    // 「インタラクトする」という実際の行動（両方のモードから呼ばれる）
    private void DoInteract()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isPlaying) return;

        // 発射地点（胸の高さ）
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;

        // 基本の方向ベクトルを計算
        Vector3 forward = transform.forward;
        Vector3 upward = transform.up * verticalAngle;
        Vector3 downward = -transform.up * verticalAngle;
        Vector3 right = transform.right * interactWidth;
        Vector3 left = -transform.right * interactWidth;

        // 5つの最終的な「発射方向」を計算
        Vector3 centerDir = forward.normalized;
        Vector3 upDir = (forward + upward).normalized;
        Vector3 downDir = (forward + downward).normalized;
        Vector3 rightDir = (forward + right).normalized;
        Vector3 leftDir = (forward + left).normalized;

        //足元を狙う用も用意
        Vector3 feetDir = (forward * 0.5f + Vector3.down * 1.0f).normalized;

        // レイキャストの優先順位
        // 正面（テーブル作業優先）
        if (CheckRay(rayOrigin, centerDir)) return;

        // 上
        if (CheckRay(rayOrigin, upDir)) return;

        // 左右（少しズレてもテーブル拾えるように）
        if (CheckRay(rayOrigin, rightDir)) return;
        if (CheckRay(rayOrigin, leftDir)) return;
        
        // 4位：斜め下（少し離れた床のもの）
        if (CheckRay(rayOrigin, downDir)) return;

        // 5位：足元（真下に落ちたもの
        if (CheckRay(rayOrigin, feetDir)) return;
    }

    // Raycastのチェック処理（GetComponentInParentを使う最終版）
    private bool CheckRay(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, interactDistance, interactableLayer))
        {
            // "Interactable" レイヤーの何かにヒットした！
            IInteracttable interactable = hit.collider.GetComponent<IInteracttable>();

            if (interactable == null)
            {
                // ヒットしたのが子オブジェクトだった場合、親を探す
                interactable = hit.collider.GetComponentInParent<IInteracttable>();
            }

            if (interactable != null)
            {
                // 見つかった！
                interactable.Interact();
                return true; // ヒットした
            }
        }
        return false; // ヒットしなかった
    }


    //アイテムを持つ関数
    public void HoldItem(GameObject itemPrefab)
    {
        if (heldItem != null) return; // 既に何か持っていたら持てない

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.sePlace);
        }

        // 手の位置にアイテムを生成
        heldItem = Instantiate(itemPrefab, handPosition);

        // 位置と回転をリセット
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        // 持ったアイテムがプレイヤーに当たって暴れないように、物理演算を無効化
        Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
        foreach (Collider c in cols)
        {
            c.enabled = false;
        }

        Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.isKinematic = true;  // 物理演算を止める
            itemRb.useGravity = false;  // 重力を切る
            itemRb.velocity = Vector3.zero;
        }

        if (anim != null)
        {
            anim.SetBool("IsHolding", true);
            anim.SetTrigger("Pick");
        }
    }


    // 外部（ボウルやゴミ箱など）から呼ばれる：持っているアイテムを渡す（消す）
    // 戻り値として「渡したアイテムの名前」を返す
    public string GiveItem()
    {
        if (heldItem == null) return ""; // 何も持っていない

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.sePlace);
        }

        // アイテムの名前を取得（"(Clone)"という文字を削除して綺麗にする）
        string itemName = heldItem.name.Replace("(Clone)", "");

        // 手元のアイテムを削除
        Destroy(heldItem);
        heldItem = null;

        if (anim != null)
        {
            anim.SetBool("IsHolding", false);
            anim.SetTrigger("Put");
        }

        return itemName;
    }

    //シーンにある実物を拾う関数
    public void PickUpItem(GameObject targetItem)
    {
        if (heldItem != null) return; // 既に何か持っていたら拾えない

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.sePlace);
        }

        heldItem = targetItem; // 持つアイテムを登録

        // 親子関係の設定や位置のリセット
        heldItem.transform.SetParent(handPosition);

        // Settingsを取得
        ItemSettings settings = heldItem.GetComponent<ItemSettings>();
        if (settings != null)
        {
            // サイズと位置の適用（既存）
            heldItem.transform.localScale = settings.onPlayerScale;
            heldItem.transform.localPosition = settings.holdPositionOffset;

            // 回転の適用
            // Quaternion.Euler で Vector3(x,y,z) を回転データに変換します
            heldItem.transform.localRotation = Quaternion.Euler(settings.onPlayerRotation);
        }
        else
        {
            // 設定がない場合はデフォルト
            heldItem.transform.localScale = Vector3.one;
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity; // 回転なし
        }

        // 物理演算と当たり判定を無効化（持っている間は暴れないように）
        Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.isKinematic = true; // 持ってるときは物理OFF
            itemRb.useGravity = false; // 重力もOFFにしておくのが安全
            itemRb.velocity = Vector3.zero; // 前の勢いを消す
        }

        Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
        foreach (Collider c in cols)
        {
            c.enabled = false;
        }

        // アニメーション更新
        if (anim != null)
        {
            anim.SetBool("IsHolding", true);
            anim.SetTrigger("Pick");
        }
    }

    public void ReleaseItem()
    {
        if (heldItem != null)
        {
            Bowl bowl = heldItem.GetComponent<Bowl>();
            if (bowl != null)
            {
                bowl.OnReleased();
            }
        }

        if (heldItem != null && AudioManager.Instance != null)
        {
            // heldItemがnullになる前にチェック
            AudioManager.Instance.PlaySE(AudioManager.Instance.sePlace);
        }

        heldItem = null; // 参照を切るだけ（オブジェクトは消さない）

        if (anim != null)
        {
            anim.SetBool("IsHolding", false);
            anim.SetTrigger("Put");
        }
    }

    public void PlayMixAnimation()
    {
        // "Mix" という名前のトリガーをONにする
        anim.SetTrigger("Mix");

        if (mixMotionCoroutine != null) StopCoroutine(mixMotionCoroutine);
        mixMotionCoroutine = StartCoroutine(MixMotionRoutine());
    }

    // 動きを制御する
    private IEnumerator MixMotionRoutine()
    {
        // --- 混ぜるモード開始 ---
        // HandPositionの親を「腕」に変更
        handPosition.SetParent(mixAnchor, false);

        // アニメーションの長さ分だけ待つ
        yield return new WaitForSeconds(0.5f);

        // --- 混ぜるモード終了 ---
        // 親を「いつもの場所」に戻す
        handPosition.SetParent(idleAnchor, false);

        mixMotionCoroutine = null;
    }

}