using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : MonoBehaviour, IInteracttable // ←スペル注意（元のままにしています）
{
    [Header("中身の表示用")]
    public GameObject contentSphere;
    private Renderer sphereRenderer;

    // --- 中身の状態 ---
    public bool hasMilk = false;
    public bool hasVanilla = false;

    // --- 調理の進行状況 ---
    public bool isMelted = false; // 溶けた
    public bool isMixed = false;  // 混ざった
    public bool isFrozen = false; // 凍った
    public bool isBurnt = false;  // 焦げた

    public bool isCracked = false;// ひび割れフラグ

    [Header("ミキサー設定")]
    public int mixClicksNeeded = 10; // 完了までに必要なクリック数
    private int currentMixClicks = 0;

    [Header("見た目の切り替え")]
    public GameObject normalModel;   // 普通のボウルのモデル（子オブジェクト）
    public GameObject crackedModel;  // ひび割れボウルのモデル（子オブジェクト）

    void Start()
    {
        if (contentSphere != null)
        {
            sphereRenderer = contentSphere.GetComponent<Renderer>();
            UpdateVisual();
        }
    }

    // 材料を入れる処理
    public bool AddIngredient(GameObject item)
    {
        if (isMelted) return false;

        // (Clone)の文字を消して名前判定
        string itemName = item.name.Replace("(Clone)", "").Trim();
        bool success = false;

        if (itemName == "Item_Milk" && !hasMilk) { hasMilk = true; success = true; }
        else if (itemName == "Item_Vanilla" && !hasVanilla) { hasVanilla = true; success = true; }

        if (success) UpdateVisual();
        return success;
    }

    // 混ぜる処理（外部から呼ばれる）
    public void AddMixProgress()
    {
        if (!IsReadyToMix()) return;

        currentMixClicks++;
        Debug.Log($"混ぜています... {currentMixClicks}/{mixClicksNeeded}");

        if (currentMixClicks >= mixClicksNeeded)
        {
            MixComplete();
        }
    }

    public void MixComplete()
    {
        isMixed = true;
        Debug.Log("ボウルの中身が混ざりました！");
        UpdateVisual();
    }

    public bool IsReadyToMix()
    {
        return isMelted && !isMixed;
    }

    // 見た目更新
    public void UpdateVisual()
    {
        if (contentSphere == null) return;

        if (!hasMilk && !hasVanilla)
        {
            contentSphere.SetActive(false);
            return;
        }
        contentSphere.SetActive(true);

        if (isBurnt) sphereRenderer.material.color = Color.black;
        else if (isFrozen) sphereRenderer.material.color = new Color(0.5f, 0.8f, 1.0f);
        else if (isMixed) sphereRenderer.material.color = new Color(1.0f, 0.95f, 0.8f);
        else if (isMelted) sphereRenderer.material.color = new Color(1.0f, 0.8f, 0.6f);
        else if (hasMilk && hasVanilla) sphereRenderer.material.color = new Color(1.0f, 0.9f, 0.7f);
        else if (hasMilk) sphereRenderer.material.color = Color.white;
        else if (hasVanilla) sphereRenderer.material.color = new Color(1.0f, 0.8f, 0.2f);
    }

    // インタラクト（直接触った場合）
    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        if (player.heldItem != null)
        {
            if (AddIngredient(player.heldItem))
            {
                GameObject item = player.heldItem;
                player.ReleaseItem();
                Destroy(item);
            }
        }
        else
        {
            player.PickUpItem(this.gameObject);
        }
    }

    // プレイヤー探索
    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 6.0f;
        foreach (var p in players) { float dist = Vector3.Distance(transform.position, p.transform.position); if (dist < minDistance) { minDistance = dist; closest = p; } }
        return closest;
    }

    public void Cook() { isMelted = true; UpdateVisual(); }
    public bool IsReadyToCook() { return hasMilk && hasVanilla && !isMelted; }
    public void Burn() { isBurnt = true; UpdateVisual(); }

    // 冷凍庫に入れてもいい状態か？（混ざっていて、まだ凍っていないならOK）
    public bool IsReadyToFreeze()
    {
        return isMixed && !isFrozen;
    }

    // 凍らせる処理（冷凍庫から呼ばれる）
    public void Freeze()
    {
        isFrozen = true;
        // 念のため他のフラグは整理してもいいですが、とりあえずそのままで
        Debug.Log("アイスが完成しました！");
        UpdateVisual();
    }

    // カップから呼ばれる「盛り付け処理」
    // 成功したら true を返す
    public bool TryScoopIceCream()
    {
        // まだ凍っていない、または既に割れていたら「失敗」
        if (!isFrozen || isCracked)
        {
            return false;
        }

        // 成功！ひび割れ状態にする
        BecomeCracked();
        return true;
    }

    // ひび割れた状態にする処理
    private void BecomeCracked()
    {
        isCracked = true;

        if (normalModel != null) normalModel.SetActive(false); // 普通のを消す
        if (crackedModel != null) crackedModel.SetActive(true); // 割れたのを出す

        Debug.Log("ボウルからアイスを取りました！ボウルはひび割れました。");
    }
}