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

    private Rigidbody rb; // 物理演算を管理するRigidbodyコンポーネント
    private Vector2 moveInput; // 移動入力（X, Y）を保持する変数


    //#if falseにすると完成版の方のデバッグができるs
#if UNITY_EDITOR // ▼▼▼ Unityエディタで実行している時だけ、この部分が有効になる ▼▼▼

    [Header("【デバッグ用】プレイヤー番号")]
    public int playerID = 1; // 【エディタ専用】プレイヤー番号（1か2）をインスペクタで指定

    // 【エディタ専用】毎フレーム、キーボード入力を直接チェックする
    void Update()
    {
        // まず入力をリセット
        moveInput = Vector2.zero;

        // playerIDに応じて、WASDか矢印キーの入力を受け取る
        if (playerID == 1)
        {
            if (Input.GetKey(KeyCode.W)) { moveInput.y = 1; }
            if (Input.GetKey(KeyCode.S)) { moveInput.y = -1; }
            if (Input.GetKey(KeyCode.A)) { moveInput.x = -1; }
            if (Input.GetKey(KeyCode.D)) { moveInput.x = 1; }
        }
        else if (playerID == 2)
        {
            if (Input.GetKey(KeyCode.UpArrow)) { moveInput.y = 1; }
            if (Input.GetKey(KeyCode.DownArrow)) { moveInput.y = -1; }
            if (Input.GetKey(KeyCode.LeftArrow)) { moveInput.x = -1; }
            if (Input.GetKey(KeyCode.RightArrow)) { moveInput.x = 1; }
        }

        if (playerID == 1 && Input.GetKeyDown(KeyCode.E)) // P1はEキー
        {
            DoInteract(); // 共通のインタラクト関数を呼ぶ
        }
        else if (playerID == 2 && Input.GetKeyDown(KeyCode.P)) // P2はPキー（など、好きなキーに）
        {
            DoInteract(); // 共通のインタラクト関数を呼ぶ
        }
    }

#else // ▼▼▼ ゲームをビルドした時（完成版）だけ、この部分が有効になる ▼▼▼

    // 【完成版】Player Inputコンポーネントからイベントとして呼び出される
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

#endif // ▲▲▲ ここで命令は終わり ▲▲▲


    // --- 両方のモードで共通して使う処理 ---

    // ゲーム開始時に一度だけ呼ばれ、自分自身のコンポーネントを取得する
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }



    // 物理演算のタイミングで一定間隔で呼ばれる
    void FixedUpdate()
    {
        // 2Dの入力を3D空間の移動ベクトルに変換する
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        // Rigidbodyの速度を更新してキャラクターを物理的に動かす（正規化して斜め移動対策）
        rb.velocity = movement.normalized * moveSpeed;

        // キャラクターの向きを入力方向に合わせる（入力がある時だけ）
        if (movement != Vector3.zero)
        {
            // 向きたい方向を計算
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            // 現在の向きから目標の向きへ、指定した速さで滑らかに回転させる
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }


    // 「インタラクトする」という実際の行動（両方のモードから呼ばれる）
    private void DoInteract()
    {
        // 1. 発射地点（胸の高さ）
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;

        // 2. 基本の方向ベクトルを計算
        Vector3 forward = transform.forward;
        Vector3 upward = transform.up * verticalAngle;
        Vector3 downward = -transform.up * verticalAngle;
        Vector3 right = transform.right * interactWidth;
        Vector3 left = -transform.right * interactWidth;

        // 3. 5つの最終的な「発射方向」を計算
        Vector3 centerDir = forward.normalized;
        Vector3 upDir = (forward + upward).normalized;
        Vector3 downDir = (forward + downward).normalized;
        Vector3 rightDir = (forward + right).normalized;
        Vector3 leftDir = (forward + left).normalized;

        // 5方向を視覚的にデバッグ表示する (Sceneビューで確認できます)
        Debug.DrawRay(rayOrigin, centerDir * interactDistance, Color.red, 1.0f);    // 真ん中 = 赤
        Debug.DrawRay(rayOrigin, upDir * interactDistance, Color.yellow, 1.0f);     // 上 = 黄色
        Debug.DrawRay(rayOrigin, downDir * interactDistance, Color.yellow, 1.0f);     // 下 = 黄色
        Debug.DrawRay(rayOrigin, rightDir * interactDistance, Color.cyan, 1.0f);      // 右 = 水色
        Debug.DrawRay(rayOrigin, leftDir * interactDistance, Color.cyan, 1.0f);      // 左 = 水色

        // 1. まずは「真ん中」を調べる
        if (CheckRay(rayOrigin, centerDir)) return;

        // 2. もしダメなら「上」を調べる
        if (CheckRay(rayOrigin, upDir)) return;

        // 3. もしダメなら「下」を調べる
        if (CheckRay(rayOrigin, downDir)) return;

        // 4. もしダメなら「右」を調べる
        if (CheckRay(rayOrigin, rightDir)) return;

        // 5. もしダメなら「左」を調べる
        if (CheckRay(rayOrigin, leftDir)) return;

        // 5本とも当たらなかった
        Debug.Log("目の前に何もない");
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

}