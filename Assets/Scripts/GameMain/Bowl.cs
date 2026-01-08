using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private bool isHeld = false;  // 持っているかどうかのフラグ

    [Header("ミキサー設定")]
    public int mixClicksNeeded = 10; // 完了までに必要なクリック数
    private int currentMixClicks = 0;

    [Header("見た目の切り替え")]
    public GameObject normalModel;   // 普通のボウルのモデル（子オブジェクト）
    public GameObject crackedModel;  // ひび割れボウルのモデル（子オブジェクト）

    [Header("UI設定（ゲージ）")]
    public GameObject gaugeObject; // Gauge2プレハブ自体
    public Slider gaugeSlider;     // その中のSlider
    public Image gaugeFillImage;

    [Header("調理時間設定")]
    public float freezeTimeNeeded = 5.0f;
    private float currentFreezeTimer = 0f;

    public float cookTimeNeeded = 5.0f; // 完成まで
    public float burnTimeNeeded = 8.0f; // 焦げるまで
    private float currentCookTimer = 0f;

    void Start()
    {
        if (contentSphere != null)
        {
            sphereRenderer = contentSphere.GetComponent<Renderer>();
            UpdateVisual();
        }

        if (gaugeObject != null) gaugeObject.SetActive(false);
    }

    // 材料を入れる処理
    public bool AddIngredient(GameObject item)
    {
        if (isBurnt || isMelted) return false;

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
        if (isBurnt) return;
        if (!IsReadyToMix()) return;

        currentMixClicks++;

        // 混ぜる時は「緑色」
        SetGaugeColor(Color.green);
        UpdateGauge((float)currentMixClicks / mixClicksNeeded);

        if (currentMixClicks >= mixClicksNeeded)
        {
            MixComplete();
            HideGauge();
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

    //凍らせる
    public void AddFreezeProgress(float deltaTime)
    {
        // 混ざってない、または既に凍ってたら無視
        if (isBurnt) return;
        if (!isMixed || isFrozen) return;

        currentFreezeTimer += deltaTime;

        // ゲージ更新
        SetGaugeColor(Color.cyan);
        UpdateGauge(currentFreezeTimer / freezeTimeNeeded);

        if (currentFreezeTimer >= freezeTimeNeeded)
        {
            Freeze();
            HideGauge(); // 終わったら隠す
        }
    }

    //温める
    public void AddCookProgress(float deltaTime)
    {
        // 焦げてたらもう何もしない
        if (isBurnt) return;
        // 材料がないなら何もしない
        if (!hasMilk || !hasVanilla) return;

        currentCookTimer += deltaTime;

        // --- フェーズ1: 調理中（0% → 100%） ---
        if (!isMelted)
        {
            // 調理中は「オレンジ」
            SetGaugeColor(new Color(0.0f, 1.0f, 0.0f));

            // ゲージを普通に増やす
            UpdateGauge(currentCookTimer / cookTimeNeeded);

            if (currentCookTimer >= cookTimeNeeded)
            {
                Cook(); // 完成！
            }
        }
        // --- フェーズ2: 焦げ進行中（ずっと100%のまま、色だけ変わる） ---
        else
        {
            // 焦げの進行度（0.0 ～ 1.0）
            float burnProgress = (currentCookTimer - cookTimeNeeded) / burnTimeNeeded;

            // ★ここが変更点！
            // ゲージの量は「1.0（満タン）」で固定！2本目は出しません。
            UpdateGauge(1.0f);

            // その代わり、色を「緑(安全) → 赤(危険)」へ徐々に変化させる
            // Color.Lerp という機能で、進行度に合わせて色を混ぜます
            SetGaugeColor(Color.Lerp(Color.green, Color.red, burnProgress));

            // 猶予時間を超えたら焦げる
            if (currentCookTimer >= (cookTimeNeeded + burnTimeNeeded))
            {
                Burn();
                HideGauge();
            }
        }
    }

    //レンジに入れる時用（見た目を消して、ゲージ更新は許可）
    public void OnPutInMicrowave()
    {
        isHeld = false; // レンジの中なので「手持ち」ではない

        // モデル（見た目）を全部消す
        ToggleModelVisibility(false);
    }

    // プレイヤーが拾った時用（見た目を戻して、ゲージは消す）
    public void OnPickedUp()
    {
        isHeld = true; // ここで確実に「持ってる」ことにする！

        // モデル（見た目）を復活させる
        ToggleModelVisibility(true);

        // ゲージは即消す
        HideGauge();
    }

    // 見た目のON/OFFを切り替える便利関数
    private void ToggleModelVisibility(bool isVisible)
    {
        // 1. 中身（液体など）
        if (contentSphere != null)
        {
            // 中身が入っている時だけ表示制御に従う
            // （材料が入ってないのに isVisible=true だからといって表示させないようにする）
            if (isVisible) UpdateVisual();
            else contentSphere.SetActive(false);
        }

        // 2. ボウルの器
        if (normalModel != null && !isCracked) normalModel.SetActive(isVisible);
        if (crackedModel != null && isCracked) crackedModel.SetActive(isVisible);

        // もしボウルの器自体に MeshRenderer がついている場合
        Renderer r = GetComponent<Renderer>();
        if (r != null) r.enabled = isVisible;
    }

    //持ったときのゲージを管理する
    public void RefreshGaugeOnPickup()
    {
        HideGauge(); // 基本は消す

        // 焦げてたら何もしない
        if (isBurnt) return;
    }

    //ゲージの表示・更新
    private void UpdateGauge(float percent)
    {
        if (isHeld)
        {
            if (gaugeObject != null && gaugeObject.activeSelf)
            {
                gaugeObject.SetActive(false);
            }
            return;
        }

        if (gaugeObject != null)
        {
            // 非表示なら表示する
            if (!gaugeObject.activeSelf) gaugeObject.SetActive(true);

            // カメラの方を向く（ビルボード処理）
            if (Camera.main != null)
            {
                gaugeObject.transform.rotation = Camera.main.transform.rotation;
            }
        }

        if (gaugeSlider != null)
        {
            gaugeSlider.value = percent; // 0.0 ～ 1.0
        }
    }

    //ゲージの色を変える
    private void SetGaugeColor(Color color)
    {
        if (gaugeFillImage != null)
        {
            gaugeFillImage.color = color;
        }
    }

    //ゲージを隠してタイマーをリセット
    private void HideGauge()
    {
        if (gaugeObject != null) gaugeObject.SetActive(false);
    }

    //プレイヤーが置く場所から呼ぶ用
    public void OnReleased()
    {
        isHeld = false;
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

            isHeld = true;

            RefreshGaugeOnPickup();
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