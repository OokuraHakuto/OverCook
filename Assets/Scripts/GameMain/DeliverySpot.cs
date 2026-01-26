using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverySpot : MonoBehaviour, IInteracttable
{
    [Header("スコア設定")]
    public int scorePerIce = 100; // 1個あたりの点数（仮）

    [Header("ナビゲーション")]
    public GameObject arrow1P;  // 1p赤矢印
    public GameObject arrow2P;  // 2p青矢印

    //　更新
    void Update()
    {
        // 矢印更新
        UpdateNavArrows();
    }

    // 矢印更新
    void UpdateNavArrows()
    {
        // 初期化
        if (arrow1P != null) arrow1P.SetActive(false);
        if (arrow2P != null) arrow2P.SetActive(false);

        // プレイヤー検索
        var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            if (p.heldItem == null) continue;

            Bowl bowl = p.heldItem.GetComponent<Bowl>();

            // ボウルを持っていて、かつ「完成品」なら
            if (bowl != null && bowl.IsFinished())
            {
                if (p.playerID == 1 && arrow1P != null) arrow1P.SetActive(true);
                if (p.playerID == 2 && arrow2P != null) arrow2P.SetActive(true);
            }
        }
    }

    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // プレイヤーが何か持っているか？
        if (player.heldItem != null)
        {
            // アイテムの名前を取得して綺麗にする
            string rawName = player.heldItem.name.Replace("(Clone)", "").Trim();

            // OrderManagerが存在するかチェック
            if (OrderManager.Instance != null)
            {
                // ここで注文マネージャーに「これ合ってる？」と聞く！
                bool isCorrectOrder = OrderManager.Instance.TryDelivery(rawName);

                if (isCorrectOrder)
                {
                    // --- 正解の処理 ---

                    // スコア加算（基本点 + 注文ボーナスなど）
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.AddScore(scorePerIce);
                    }

                    // アイテムを消す
                    player.GiveItem();

                    // 成功エフェクトや音など
                    Debug.Log("納品成功！");
                }
                else
                {
                    // --- 間違い ---
                }
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
