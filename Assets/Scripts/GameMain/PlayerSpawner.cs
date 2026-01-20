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
                // 生成したP1オブジェクトを変数に格納
                GameObject player1Object = Instantiate(p1Prefab, player1SpawnPoint.position, player1SpawnPoint.rotation);

                // P1オブジェクトからPlayerControllerスクリプトを取得
                PlayerController p1Controller = player1Object.GetComponent<PlayerController>();
                if (p1Controller != null)
                {
                    // playerIDを「1」に設定
                    p1Controller.playerID = 1;
                }
            }
            
            // ---------------------------------
            // プレイヤー2をスポーンさせる
            // ---------------------------------
            GameObject p2Prefab = SelectionManager.instance.player2Prefab;
            if (p2Prefab != null)
            {
                // 生成したP2オブジェクトを変数に格納
                GameObject player2Object = Instantiate(p2Prefab, player2SpawnPoint.position, player2SpawnPoint.rotation);

                // P2オブジェクトからPlayerControllerスクリプトを取得
                PlayerController p2Controller = player2Object.GetComponent<PlayerController>();
                if (p2Controller != null)
                {
                    // playerIDを「2」に設定
                    p2Controller.playerID = 2;
                }
            }
        }
    }
}