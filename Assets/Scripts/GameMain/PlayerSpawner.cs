using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("プレイヤー1の出現位置")]
    public Transform player1SpawnPoint; // P1がスポーンする場所

    [Header("プレイヤー2の出現位置")]
    public Transform player2SpawnPoint; // P2がスポーンする場所

    void Start()
    {
        if (SelectionManager.instance != null)
        {
            // ---------------------------------
            // プレイヤー1をスポーンさせる
            // ---------------------------------
            GameObject p1Prefab = SelectionManager.instance.player1Prefab;
            if (p1Prefab != null)
            {
                // 1. 生成したP1オブジェクトを変数に格納
                GameObject player1Object = Instantiate(p1Prefab, player1SpawnPoint.position, player1SpawnPoint.rotation);

                // 2. P1オブジェクトからPlayerControllerスクリプトを取得
                PlayerController p1Controller = player1Object.GetComponent<PlayerController>();
                if (p1Controller != null)
                {
                    // 3. デバッグ用のplayerIDを「1」に設定
                    p1Controller.playerID = 1;
                }
                Debug.Log("プレイヤー1: " + p1Prefab.name + " をスポーンしました (ID: 1)");
            }
            else
            {
                Debug.LogWarning("プレイヤー1のキャラクターが選択されていません！");
            }

            // ---------------------------------
            // プレイヤー2をスポーンさせる
            // ---------------------------------
            GameObject p2Prefab = SelectionManager.instance.player2Prefab;
            if (p2Prefab != null)
            {
                // 1. 生成したP2オブジェクトを変数に格納
                GameObject player2Object = Instantiate(p2Prefab, player2SpawnPoint.position, player2SpawnPoint.rotation);

                // 2. P2オブジェクトからPlayerControllerスクリプトを取得
                PlayerController p2Controller = player2Object.GetComponent<PlayerController>();
                if (p2Controller != null)
                {
                    // 3. デバッグ用のplayerIDを「2」に設定
                    p2Controller.playerID = 2;
                }
                Debug.Log("プレイヤー2: " + p2Prefab.name + " をスポーンしました (ID: 2)");
            }
            else
            {
                Debug.LogWarning("プレイヤー2のキャラクターが選択されていません！");
            }
        }
        else
        {
            Debug.LogError("キャラクター選択情報（SelectionManager）が見つかりません！");
        }
    }
}