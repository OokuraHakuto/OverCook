using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour, IInteracttable
{
    public void Interact()
    {
        // 近くのプレイヤーを探す
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // プレイヤーが何か持っていたら
        if (player.heldItem != null)
        {
            // PlayerControllerにある「GiveItem」を呼ぶだけでOK！
            // このメソッドが「手持ちアイテムの削除」と「アニメーションのリセット」を全部やってくれる。
            string trashedItemName = player.GiveItem();

            Debug.Log(trashedItemName + " をゴミ箱に捨てました！");
        }
    }

    // プレイヤー探索
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
