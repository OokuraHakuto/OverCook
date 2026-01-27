using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverySpot : MonoBehaviour, IInteracttable
{
    [Header("★スコア設定 (Easy)")]
    public int baseScoreEasy = 100;
    public int maxBonusEasy = 100; // 合計Max 200

    [Header("★スコア設定 (Normal)")]
    public int baseScoreNormal = 150;
    public int maxBonusNormal = 200; // 合計Max 350

    [Header("★スコア設定 (Hard)")]
    public int baseScoreHard = 250;
    public int maxBonusHard = 350; // 合計Max 600！

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

            Cup cup = p.heldItem.GetComponent<Cup>();

            // 「カップ」を持っていて、かつ「中身が入っている（完成品）」なら
            if (cup != null && cup.isFull)
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
                // outパラメータで残り時間割合を受け取る変数を用意
                float timeRatio = 0f;

                // TryDeliveryの呼び出し
                bool isCorrectOrder = OrderManager.Instance.TryDelivery(rawName, out timeRatio);

                if (isCorrectOrder)
                {
                    // 難易度に応じてスコア配分を決める
                    int currentBase = baseScoreNormal;
                    int currentBonus = maxBonusNormal;

                    if (SelectionManager.instance != null)
                    {
                        int diff = SelectionManager.instance.difficulty;
                        switch (diff)
                        {
                            case 0: // Easy
                                currentBase = baseScoreEasy;
                                currentBonus = maxBonusEasy;
                                break;
                            case 1: // Normal
                                currentBase = baseScoreNormal;
                                currentBonus = maxBonusNormal;
                                break;
                            case 2: // Hard
                                currentBase = baseScoreHard;
                                currentBonus = maxBonusHard;
                                break;
                        }
                    }

                    // 選ばれた設定値を使って計算
                    int finalScore = currentBase + Mathf.RoundToInt(currentBonus * timeRatio);

                    // スコア加算
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.AddScore(finalScore);
                    }

                    // アイテムを消す
                    player.GiveItem();
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
