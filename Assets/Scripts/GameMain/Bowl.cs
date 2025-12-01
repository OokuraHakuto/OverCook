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

        // 色の分岐（仮）
        if (hasMilk && hasVanilla)
        {
            // 両方入った（レンチン準備OK）
            sphereRenderer.material.color = Color.white;
        }
        else if (hasMilk)
        {
            // 牛乳だけ
            sphereRenderer.material.color = new Color(0.9f, 0.9f, 1f);
        }
        else if (hasVanilla)
        {
            // バニラだけ
            sphereRenderer.material.color = new Color(1f, 1f, 0.6f);
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
}