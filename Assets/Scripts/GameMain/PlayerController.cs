using UnityEngine;
// 完成版（ビルド時）でのみInput Systemを読み込む
#if !UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class PlayerController : MonoBehaviour
{
    // --- 両方のモードで共通して使う変数 ---
    [Header("移動速度")]
    public float moveSpeed = 5f; // キャラクターの移動速度
    [Header("回転の速さ")]
    public float rotateSpeed = 10f; // キャラクターの回転の追従速度

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
    }

#else // ▼▼▼ ゲームをビルドした時（完成版）だけ、この部分が有効になる ▼▼▼

    // 【完成版】Player Inputコンポーネントからイベントとして呼び出される
    public void OnMove(InputAction.CallbackContext context)
    {
        // パッドやキーボードからの入力をVector2として受け取る
        moveInput = context.ReadValue<Vector2>();
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
}