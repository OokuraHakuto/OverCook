using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCrate : MonoBehaviour, IInteracttable
{
    public enum CrateType
    {
        Milk,    // 牛乳
        Essence  // エッセンス（バニラ、チョコなど全部これ）
    }

    [Header("箱の設定")]
    public CrateType crateType;

    [Header("ここから取れるアイテムの設定")]
    public GameObject itemPrefab; // 手に持たせる時のプレファブ（牛乳パックなど）
    public string ingredientName; // "Milk" や "Vanilla" など（ボウルに渡す名前）

    [Header("ナビゲーション")]
    public GameObject arrow1P;  // 1p赤矢印
    public GameObject arrow2P;  // 2p青矢印

    // 更新
    void Update()
    {
        // 矢印の更新処理
        UpdateNavArrows();
    }

    // 矢印の更新
    void UpdateNavArrows()
    {
        // 一旦消す
        if (arrow1P != null) arrow1P.SetActive(false);
        if (arrow2P != null) arrow2P.SetActive(false);

        // プレイヤーを全検索
        var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            // 何も持っていなければ次へ
            if (p.heldItem == null) continue;

            // 持っているのがボウルじゃなければ次へ
            Bowl bowl = p.heldItem.GetComponent<Bowl>();
            if (bowl == null) continue;

            // 判定関数呼び出し「この箱の材料がボウルに必要か？」
            if (CheckIfBowlNeedsMe(bowl))
            {
                // 必要なら矢印をONにする
                if (p.playerID == 1 && arrow1P != null) arrow1P.SetActive(true);
                if (p.playerID == 2 && arrow2P != null) arrow2P.SetActive(true);
            }
        }
    }

    // ボウルがこの箱の材料を欲しがっているか判定する関数
    bool CheckIfBowlNeedsMe(Bowl bowl)
    {
        // そもそも調理が進んでしまっていたら不要
        if (bowl.isMelted || bowl.isFrozen || bowl.isBurnt) return false;

        // 箱のタイプによって判定を変える
        if (crateType == CrateType.Milk)
        {
            return bowl.NeedsMilk(); // ボウル側の関数「牛乳足りない？」
        }
        else if (crateType == CrateType.Essence)
        {
            return bowl.NeedsEssence(); // ボウル側の関数「味足りない？」
        }

        return false;
    }

    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // ---------------------------------------------------------
        // パターンA：プレイヤーが「何か」を持っている場合
        // ---------------------------------------------------------
        if (player.heldItem != null)
        {
            // 持っているアイテムに「Bowl」スクリプトがついているか調べる
            Bowl heldBowl = player.heldItem.GetComponent<Bowl>();

            if (heldBowl != null)
            {
                if (heldBowl != null)
                {
                    // itemPrefabを直接渡す
                    bool added = heldBowl.AddIngredient(itemPrefab);
                }
            }
            else
            {
                // 持っているのが「ボウル以外（ただの牛乳など）」だった場合
                // 手がふさがっている
            }
        }
        // ---------------------------------------------------------
        // パターンB：プレイヤーが「手ぶら」の場合
        // ---------------------------------------------------------
        else
        {
            // → そのまま材料を手に持つ
            player.HoldItem(itemPrefab);
            Debug.Log(ingredientName + " を持ちました！");
        }
    }

    // プレイヤー探索
    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 7.0f;

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
