using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupProvider : MonoBehaviour, IInteracttable
{
    [Header("渡すアイテムのプレハブ")]
    public GameObject cupPrefab; // ここに Cup_Empty を入れる

    [Header("ナビゲーション")]
    public GameObject arrow1P;  // 1p赤矢印
    public GameObject arrow2P;  // 2p青矢印


    // 更新
    void Update()
    {
        UpdateNavArrows();
    }

    // 矢印制御
    void UpdateNavArrows()
    {
        // 一旦リセット
        if (arrow1P != null) arrow1P.SetActive(false);
        if (arrow2P != null) arrow2P.SetActive(false);

        // 「机の上に放置された冷凍ボウル」があるか探す
        bool needsCup = CheckIfFrozenBowlOnTableExists();

        if (!needsCup) return; // 必要なければ誰も呼ばない

        // 手ぶらのプレイヤーを探して呼ぶ
        var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            // 手ぶらなら「カップ取りに来い！」と矢印を出す
            if (p.heldItem == null)
            {
                if (p.playerID == 1 && arrow1P != null) arrow1P.SetActive(true);
                if (p.playerID == 2 && arrow2P != null) arrow2P.SetActive(true);
            }
        }
    }

    // シーン全体から誰にも持たれていない冷凍ボウルを探し出す
    bool CheckIfFrozenBowlOnTableExists()
    {
        // 全ボウルを検索
        Bowl[] allBowls = FindObjectsByType<Bowl>(FindObjectsSortMode.None);

        foreach (var bowl in allBowls)
        {
            // 条件：凍っている ＆ 焦げてない
            if (bowl.isFrozen && !bowl.isBurnt)
            {
                // さらに「誰にも持たれていない（親がプレイヤーじゃない）」かチェック
                // GetComponentInParentで親を遡ってPlayerControllerを探す
                PlayerController holder = bowl.GetComponentInParent<PlayerController>();

                // プレイヤーが持っていない ＝ 机にある
                if (holder == null)
                {
                    return true; // 1個でもあればOK
                }
            }
        }
        return false;
    }

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
