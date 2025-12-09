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
        // パターンA：台に何か置いてある場合 → プレイヤーが取る
        // =========================================================
        if (heldItem != null)
        {
            // プレイヤーが手ぶらなら
            if (player.heldItem == null)
            {
                // コライダーを復活させてから渡す
                Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
                foreach (Collider c in cols) c.enabled = true;

                player.PickUpItem(heldItem);
                heldItem = null; // 台は空になる
                Debug.Log("台からアイテムを取りました");
            }
            else
            {
                Debug.Log("手がふさがっています！");
                // ※将来的に、ここで「皿と食材を合体させる」処理などを追加できます
            }
        }
        // =========================================================
        // パターンB：台が空の場合 → プレイヤーが置く
        // =========================================================
        else
        {
            // プレイヤーが何か持っているなら
            if (player.heldItem != null)
            {
                // 手放させる（参照を切る）
                GameObject itemToPlace = player.heldItem;
                player.ReleaseItem();

                // 台の上に移動
                itemToPlace.transform.SetParent(holdPoint);
                itemToPlace.transform.localPosition = Vector3.zero;
                itemToPlace.transform.localRotation = Quaternion.identity;

                // スケールを元に戻す（念のため）
                itemToPlace.transform.localScale = Vector3.one;

                // ★重要：置いてある間もインタラクトできるようにコライダーを復活させる
                Collider[] cols = itemToPlace.GetComponentsInChildren<Collider>();
                foreach (Collider c in cols) c.enabled = true;

                // 台が記憶する
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
