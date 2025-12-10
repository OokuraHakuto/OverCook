using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCrate : MonoBehaviour, IInteracttable
{
    [Header("ここから取れるアイテムの設定")]
    public GameObject itemPrefab; // 手に持たせる時のプレファブ（牛乳パックなど）
    public string ingredientName; // "Milk" や "Vanilla" など（ボウルに渡す名前）

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
                // ★持っているのが「ボウル」だった場合
                // → ボウルの中に材料を入れる
                bool added = heldBowl.AddIngredient(itemPrefab);

                if (added) Debug.Log("ボウルに " + ingredientName + " を入れました！");
                else Debug.Log("そのボウルには入りません");
            }
            else
            {
                // ★持っているのが「ボウル以外（ただの牛乳など）」だった場合
                // → 「手がふさがってるよ！ボウルがないと混ぜられないよ！」と拒否する
                Debug.Log("手がふさがっています！材料を2個持つにはボウルが必要です。");
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

    // (FindClosestPlayerはそのまま)
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
