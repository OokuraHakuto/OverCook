using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Freezer : MonoBehaviour, IInteracttable
{
    [Header("設定")]
    public Transform holdPoint;       // ボウルを置く場所

    // 内部変数
    private GameObject heldItem;
    private Bowl heldBowl; // 中に入っているボウル

    void Update()
    {
        // ボウルが入っているなら、ボウル自身の「冷凍進行処理」を呼び出す
        if (heldBowl != null)
        {
            // Time.deltaTime（1フレームの時間）を渡して、ボウル側で計算してもらう
            heldBowl.AddFreezeProgress(Time.deltaTime);
        }
    }

    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // --- パターンA：中に物がある（取り出す） ---
        if (heldItem != null)
        {
            if (player.heldItem == null)
            {
                if (heldBowl != null)
                {
                    heldBowl.OnPickedUp();
                }

                // プレイヤーに渡す
                player.PickUpItem(heldItem);

                // アイテム設定リセット（大きさなどをプレイヤー用に戻す）
                ItemSettings settings = heldItem.GetComponent<ItemSettings>();
                if (settings != null)
                {
                    heldItem.transform.localScale = settings.onPlayerScale;
                    heldItem.transform.localPosition = settings.holdPositionOffset;
                    heldItem.transform.localRotation = Quaternion.Euler(settings.onPlayerRotation);
                }

                heldItem = null;
                heldBowl = null;
            }
        }
        // --- パターンB：空っぽ（入れる） ---
        else
        {
            if (player.heldItem != null)
            {
                Bowl bowl = player.heldItem.GetComponent<Bowl>();

                // 条件チェック：ボウルを持っていて、かつ「冷凍できる状態」か？
                if (bowl != null && bowl.IsReadyToFreeze())
                {
                    // プレイヤーから受け取る
                    heldItem = player.heldItem;
                    player.ReleaseItem();

                    // 冷凍庫に配置
                    heldItem.transform.SetParent(holdPoint);
                    heldItem.transform.localPosition = Vector3.zero;
                    heldItem.transform.localRotation = Quaternion.identity;

                    // スケール調整
                    ItemSettings settings = heldItem.GetComponent<ItemSettings>();
                    if (settings != null) heldItem.transform.localScale = settings.onTableScale;

                    heldBowl = bowl;
                }
            }
        }
    }

    // プレイヤー探索（共通）
    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 10.0f;
        foreach (var p in players) { float dist = Vector3.Distance(transform.position, p.transform.position); if (dist < minDistance) { minDistance = dist; closest = p; } }
        return closest;
    }
}
