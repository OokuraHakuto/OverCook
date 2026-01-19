using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePickup : MonoBehaviour, IInteracttable
{
    //誰でも拾えるようにする
    public void Interact()
    {
        // 一番近くのプレイヤーを探す
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // プレイヤーが手ぶらなら
        if (player.heldItem == null)
        {
            // 3. 自分自身を拾わせる
            player.PickUpItem(this.gameObject);
        }
    }

    // 近くのプレイヤーを探す関数
    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 5.0f; // 近くのみ
        foreach (var p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = p;
            }
        }
        return closest;
    }
}
