using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupProvider : MonoBehaviour, IInteracttable
{
    [Header("渡すアイテムのプレハブ")]
    public GameObject cupPrefab; // ここに Cup_Empty を入れる

    public void Interact()
    {
        // 近くのプレイヤーを探す
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // プレイヤーが「手ぶら」ならカップを渡す
        if (player.heldItem == null)
        {
            // カップを生成
            GameObject newCup = Instantiate(cupPrefab);

            // プレイヤーに持たせる
            player.PickUpItem(newCup);

            // 持ち方（位置・サイズ）の調整
            ItemSettings settings = newCup.GetComponent<ItemSettings>();
            if (settings != null)
            {
                newCup.transform.localScale = settings.onPlayerScale;
                newCup.transform.localPosition = settings.holdPositionOffset;
                newCup.transform.localRotation = Quaternion.Euler(settings.onPlayerRotation);
            }
        }
        else
        {
            //手がふさがっている
        }
    }

    // プレイヤー探索
    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 3.0f; // 届く範囲
        foreach (var p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDistance) { minDistance = dist; closest = p; }
        }
        return closest;
    }
}
