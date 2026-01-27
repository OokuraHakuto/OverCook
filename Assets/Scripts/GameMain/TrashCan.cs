using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour, IInteracttable
{
    [Header("ナビゲーション")]
    public GameObject arrow1P;  // 1p赤矢印
    public GameObject arrow2P;  // 2p青矢印


    // 更新
    void Update()
    {
        UpdateNavArrows();
    }

    //矢印更新
    void UpdateNavArrows()
    {
        // 初期化
        if (arrow1P != null) arrow1P.SetActive(false);
        if (arrow2P != null) arrow2P.SetActive(false);

        // プレイヤー検索
        var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            // 何も持っていなければスルー
            if (p.heldItem == null) continue;

            // 持っているのがボウルかチェック
            Bowl bowl = p.heldItem.GetComponent<Bowl>();

            // 条件：「ボウルを持っていて」かつ「捨てるべき状態（ひび割れor焦げ）」なら
            if (bowl != null && bowl.NeedsDisposal())
            {
                if (p.playerID == 1 && arrow1P != null) arrow1P.SetActive(true);
                if (p.playerID == 2 && arrow2P != null) arrow2P.SetActive(true);
            }
        }
    }

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
