using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour, IInteracttable
{
    [Header("設定")]
    public Transform holdPoint; // アイテムを置く位置

    private GameObject heldItem; // 今乗っているアイテム（ボウルに限らず何でも）

    [Header("制限設定")]
    [Tooltip("チェックを外すと、プレイヤーはこの台にアイテムを置けなくなります（拾う専用になる）")]
    public bool canPlaceItem = true; // デフォルトは「置ける」

    // ゲーム開始時、最初から乗っているアイテムを認識
    void Start()
    {
        if (holdPoint.childCount > 0)
        {
            heldItem = holdPoint.GetChild(0).gameObject;
            // 最初からあるアイテムのコライダーはオフにしておく（机がクリックしやすいように）
            Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
            foreach (Collider c in cols) c.enabled = false;
        }
    }

    public void Interact()
    {
        // プレイヤーを探す
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // =========================================================
        // パターンA：台に何か置いてある場合
        // =========================================================
        if (heldItem != null)
        {
            // --- ケース1：プレイヤーが手ぶら（アイテムを拾う） ---
            if (player.heldItem == null)
            {
                // コライダーを復活
                Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
                foreach (Collider c in cols) c.enabled = true;

                player.PickUpItem(heldItem);

                // ★スケール修正：拾う前に「手持ち時のスケール」に戻す
                ItemSettings settings = heldItem.GetComponent<ItemSettings>();
                if (settings != null)
                {
                    // 設定された「持った時のサイズ」にする
                    heldItem.transform.localScale = settings.onPlayerScale;
                }
                else
                {
                    // 設定がついてないアイテムならとりあえず (1,1,1) に戻す
                    heldItem.transform.localScale = Vector3.one;
                }

                heldItem = null;
                Debug.Log("台からアイテムを取りました");
            }
            // --- ケース2：プレイヤーが何か持っている（合体・投入処理） ---
            else
            {
                // 机に乗っているのがボウルか確認
                Bowl bowl = heldItem.GetComponent<Bowl>();

                // ★追加：持っているのが「泡だて器」かどうかチェック
                Whisk whisk = player.heldItem.GetComponent<Whisk>();

                // ---------------------------------------------------------
                // パターンA：泡だて器を持っていて、机にボウルがある場合 → 「混ぜる」
                // ---------------------------------------------------------
                if (whisk != null && bowl != null)
                {
                    // 混ぜられる状態（溶けている＆まだ混ざってない）か確認
                    if (bowl.IsReadyToMix())
                    {
                        // 混ぜる処理を実行（1回カウントを進める）
                        bowl.AddMixProgress();

                        // ※ここに「泡だて器を振るアニメーション」などを入れると完璧
                    }
                    else
                    {
                        if (bowl.isMixed) Debug.Log("もう混ざっています");
                        else Debug.Log("まだ溶けていません（レンジへ！）");
                    }
                }
                // ---------------------------------------------------------
                // パターンB：泡だて器以外のものを持っている場合 → 「食材を入れる」
                // ---------------------------------------------------------
                else if (bowl != null)
                {
                    // (GameObjectをそのまま渡す)
                    bool success = bowl.AddIngredient(player.heldItem);

                    if (success)
                    {
                        // 成功したらプレイヤーの手元からアイテムを消す
                        GameObject ingredient = player.heldItem;
                        player.ReleaseItem(); // プレイヤーの手を空にする
                        Destroy(ingredient);  // オブジェクトを破壊

                        Debug.Log("調理アクション成功：ボウルに入れました");
                        return; // ここで処理終了
                    }
                }

                // どちらでもない場合
                Debug.Log("手がふさがっており、アクションできませんでした！");
            }
        }
        // =========================================================
        // パターンB：台が空の場合 → プレイヤーが置く
        // =========================================================
        else
        {
            // 置けない設定ならここで弾く
            if (!canPlaceItem) return;

            if (player.heldItem != null)
            {
                GameObject itemToPlace = player.heldItem;
                player.ReleaseItem();

                itemToPlace.transform.SetParent(holdPoint);
                itemToPlace.transform.localPosition = Vector3.zero;
                itemToPlace.transform.localRotation = Quaternion.identity;

                // ★スケール修正：机に置くときのサイズを適用
                ItemSettings settings = itemToPlace.GetComponent<ItemSettings>();
                if (settings != null)
                {
                    itemToPlace.transform.localScale = settings.onTableScale;
                }
                else
                {
                    itemToPlace.transform.localScale = Vector3.one;
                }

                // 机の上にある間は、誤操作防止のためコライダーを切るのが一般的ですが
                // 復活させたいとのことなので、ここはそのままにします
                Collider[] cols = itemToPlace.GetComponentsInChildren<Collider>();
                foreach (Collider c in cols) c.enabled = true;

                heldItem = itemToPlace;
                Debug.Log("台に置きました");
            }
        }
    }

    // プレイヤー探索
    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 10.0f;
        foreach (var p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDistance) { minDistance = dist; closest = p; }
        }
        return closest;
    }
}
