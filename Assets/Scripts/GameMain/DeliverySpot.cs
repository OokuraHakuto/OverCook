using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverySpot : MonoBehaviour, IInteracttable
{
    [Header("スコア設定")]
    public int scorePerIce = 100; // 1個あたりの点数（仮）

    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // プレイヤーが何か持っているか？
        if (player.heldItem != null)
        {
            // 持っているのが「カップ」かチェック
            Cup cup = player.heldItem.GetComponent<Cup>();

            // カップで、かつ「中身が入っている(完成品)」なら納品成功！
            if (cup != null && cup.isFull)
            {
                // 1. スコアを加算
                // GameManagerが存在する場合のみ実行
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(scorePerIce);
                }

                // 2. アイテムを消す（納品完了）
                // PlayerControllerのGiveItemで綺麗に消してもらう
                string itemName = player.GiveItem();

                // (オプション) 納品成功音とかエフェクトをここで出す
                Debug.Log(itemName + " を納品しました！");
            }
            else
            {
                // 完成品じゃない（空のカップやボウルなど）
                Debug.Log("それは納品できません（完成したアイスを持ってきてね）");
            }
        }
    }

    // いつものプレイヤー探索
    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 3.0f;
        foreach (var p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDistance) { minDistance = dist; closest = p; }
        }
        return closest;
    }
}
