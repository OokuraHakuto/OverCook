using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : MonoBehaviour, IInteracttable
{
    [Header("中身の表示用")]
    public GameObject contentSphere;
    private Renderer sphereRenderer;

    // --- 中身の状態 ---
    public bool hasMilk = false;
    public bool hasVanilla = false;

    // --- 調理の進行状況 ---
    public bool isMelted = false; // レンチン完了
    public bool isMixed = false;  // 混ぜ完了
    public bool isFrozen = false; // 冷凍完了（完成）
    public bool isBurnt = false;  // 焦げフラグ

    void Start()
    {
        if (contentSphere != null)
        {
            sphereRenderer = contentSphere.GetComponent<Renderer>();
            UpdateVisual();
        }
    }

    // ▼▼▼ 追加：材料を受け入れる関数 ▼▼▼
    public bool AddIngredient(string ingredient)
    {
        // 既にレンチン済みなら、もう材料は足せない
        if (isMelted) return false;

        bool success = false;

        if (ingredient == "Milk" && !hasMilk)
        {
            hasMilk = true;
            success = true;
        }
        else if (ingredient == "Vanilla" && !hasVanilla)
        {
            hasVanilla = true;
            success = true;
        }

        if (success)
        {
            UpdateVisual();
        }
        return success; // 「入れたよ」か「入れられなかったよ」を返す
    }

    // プレイヤーが手ぶらでインタラクトしたら、ボウルを持たせる
    public void Interact()
    {
        Debug.Log("BowlのInteractが呼ばれました"); // 動作確認用ログ

        PlayerController player = FindClosestPlayer();

        if (player == null)
        {
            Debug.LogError("プレイヤーが見つかりません（距離が遠い？）");
            return;
        }

        // ---------------------------------------------------------
        // パターンA：プレイヤーが「何か」を持っている場合 → ボウルに入れる
        // ---------------------------------------------------------
        if (player.heldItem != null)
        {
            // プレイヤーからアイテムを受け取って（消して）、名前を取得
            string itemName = player.GiveItem();
            Debug.Log("投入されたアイテム名: " + itemName);

            if (itemName == "Item_Milk") // ※プレファブ名と完全一致させる！
            {
                hasMilk = true;
                Debug.Log("ボウルに牛乳が入りました！");
                UpdateVisual();
            }
            else if (itemName == "Item_Vanilla") // ※プレファブ名と完全一致させる！
            {
                hasVanilla = true;
                Debug.Log("ボウルにバニラが入りました！");
                UpdateVisual();
            }
            else
            {
                Debug.LogWarning("そのアイテム (" + itemName + ") はボウルに入れられません");
            }
        }
        // ---------------------------------------------------------
        // パターンB：プレイヤーが「手ぶら」の場合 → ボウルを拾う
        // ---------------------------------------------------------
        else
        {
            player.PickUpItem(this.gameObject);
            Debug.Log("ボウルを拾いました！");
        }
    }

    // 混ぜ完了時に呼ばれる関数
    public void MixComplete()
    {
        isMixed = true;
        Debug.Log("ボウルの中身が混ざりました！");
        UpdateVisual();
    }

    // 混ぜる工程に進んでいいかどうか
    public bool IsReadyToMix()
    {
        // 溶けていて(isMelted)、まだ混ぜていない(isMixedがfalse)ならOK
        return isMelted && !isMixed;
    }

    // 見た目の更新
    public void UpdateVisual()
    {
        if (contentSphere == null) return;

        if (!hasMilk && !hasVanilla)
        {
            contentSphere.SetActive(false); // 空っぽ
            return;
        }

        contentSphere.SetActive(true);

        // 優先順位： 焦げ > 冷凍(完成) > 混ぜ(New!) > 溶け > 材料
        if (isBurnt)
        {
            sphereRenderer.material.color = Color.black;
        }
        else if (isFrozen) // ※冷凍はまだ作ってませんが場所だけ
        {
            sphereRenderer.material.color = new Color(0.5f, 0.8f, 1.0f); // アイスっぽい色
        }
        else if (isMixed)
        {
            // 混ぜると少し白っぽく、ふんわりした色になるイメージ
            sphereRenderer.material.color = new Color(1.0f, 0.95f, 0.8f);
        }
        else if (isMelted)
        {
            // 溶けた色（例：茶色っぽくする、または濃いクリーム色）
            sphereRenderer.material.color = new Color(1.0f, 0.8f, 0.6f);
        }
        else if (hasMilk && hasVanilla)
        {
            // 準備OKの色（白）
            sphereRenderer.material.color = new Color(1.0f, 0.9f, 0.7f);
        }
        else if (hasMilk)
        {
            sphereRenderer.material.color = Color.white;
        }
        else if (hasVanilla)
        {
            sphereRenderer.material.color = new Color(1.0f, 0.8f, 0.2f);
        }
    }

    // (FindClosestPlayerは省略...)
    private PlayerController FindClosestPlayer() 
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 6.0f;

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

    public void Cook()
    {
        isMelted = true; // 溶けたフラグON
        Debug.Log("ボウルの中身が溶けました！");
        UpdateVisual(); // 見た目更新
    }

    public bool IsReadyToCook()
    {
        // 牛乳とバニラの両方が入っていて、まだ溶けてなければOK
        // (将来的に味が増えたら、ここを「材料カウント >= 2」などに変えればOK)
        return hasMilk && hasVanilla && !isMelted;
    }

    public void Burn()
    {
        isBurnt = true;
        // isMelted = false; // 焦げたら「溶けた」扱いではなく「失敗」扱いにするなら
        Debug.Log("真っ黒焦げだ！！");
        UpdateVisual();
    }


}