using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // インスペクタからPlayerInputManagerをセットする
    public PlayerInputManager playerInputManager;

    // このスクリプトが有効になった時に呼ばれる
    private void OnEnable()
    {
        // PlayerInputManagerの「プレイヤーが参加した時」のイベントに関数を登録する
        playerInputManager.onPlayerJoined += OnPlayerJoined;
    }

    // このスクリプトが無効になった時に呼ばれる
    private void OnDisable()
    {
        // 登録した関数を解除する（お作法）
        playerInputManager.onPlayerJoined -= OnPlayerJoined;
    }

    void Update()
    {
        // 「1」キーが押されたら
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // プレイヤー1 (Keyboard_P1操作) として参加させる
            playerInputManager.JoinPlayer(playerIndex: 0, controlScheme: "Keyboard_P1");
        }

        // 「2」キーが押されたら
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // プレイヤー2 (Keyboard_P2操作) として参加させる
            playerInputManager.JoinPlayer(playerIndex: 1, controlScheme: "Keyboard_P2");
        }
    }

    // ★★★ 新しく追加した部分 ★★★
    // プレイヤーが参加した瞬間に、PlayerInputManagerによって自動的に呼び出される関数
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("プレイヤーが参加しました: " + playerInput.playerIndex);

        // 参加したプレイヤーが使っているControl Schemeをチェック
        if (playerInput.currentControlScheme == "Keyboard_P1")
        {
            // もしP1の操作方法なら、"Player"マップ（WASD）を有効にする
            Debug.Log("操作マップ 'Player' を設定します");
            playerInput.SwitchCurrentActionMap("Player");
        }
        else if (playerInput.currentControlScheme == "Keyboard_P2")
        {
            // もしP2の操作方法なら、"Player2"マップ（矢印キー）を有効にする
            Debug.Log("操作マップ 'Player2' を設定します");
            playerInput.SwitchCurrentActionMap("Player2");
        }
    }
}