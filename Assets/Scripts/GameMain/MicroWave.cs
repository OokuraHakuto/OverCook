using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrWave : MonoBehaviour, IInteracttable
{
    [Header("設定")]
    public Transform holdPoint;   // ボウルを置く場所

    private GameObject heldItem;
    private Bowl heldBowl;        // 今中に入っているボウル

    void Update()
    {
        // ボウルが入っているなら
        if (heldBowl != null)
        {
            // 更新前の状態を覚えておく
            bool wasMelted = heldBowl.isMelted;

            // 加熱処理
            heldBowl.AddCookProgress(Time.deltaTime);

            // 溶けたかの判定
            if (!wasMelted && heldBowl.isMelted)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySE(AudioManager.Instance.seRange);
                }
            }
        }
    }

    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // ---------------------------------------------------------
        // パターンA：レンジにボウルがある（取り出す）
        // ---------------------------------------------------------
        if (heldItem != null)
        {
            if (player.heldItem == null) // 手ぶらなら
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySE(AudioManager.Instance.sePlace);
                }

                // プレイヤーに渡す
                player.PickUpItem(heldItem);

                // アイテム設定リセット
                ItemSettings settings = heldItem.GetComponent<ItemSettings>();
                if (settings != null)
                {
                    heldItem.transform.localScale = settings.onPlayerScale;
                    heldItem.transform.localPosition = settings.holdPositionOffset;
                    heldItem.transform.localRotation = Quaternion.Euler(settings.onPlayerRotation);
                }

                if (heldBowl != null)
                {
                    heldBowl.OnPickedUp();
                }

                // リセット
                heldItem = null;
                heldBowl = null;
            }
        }
        // =========================================================
        // パターンB：レンジが空 → ボウルを入れる
        // =========================================================
        else
        {
            if (player.heldItem != null)
            {
                Bowl bowl = player.heldItem.GetComponent<Bowl>();

                // 温められる状態かチェック
                if (bowl != null && bowl.IsReadyToCook())
                {
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySE(AudioManager.Instance.sePlace);
                    }

                    heldItem = player.heldItem;
                    player.ReleaseItem(); // 手放す

                    // レンジの中に移動
                    heldItem.transform.SetParent(holdPoint);
                    heldItem.transform.localPosition = Vector3.zero;
                    heldItem.transform.localRotation = Quaternion.identity;

                    heldItem.transform.localScale = Vector3.one;

                    // スケール調整
                    ItemSettings settings = heldItem.GetComponent<ItemSettings>();
                    if (settings != null) heldItem.transform.localScale = settings.onTableScale;

                    // 物理演算を止める
                    Rigidbody rb = heldItem.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.useGravity = false;
                    }

                    // コライダーを復活させる（念のため）
                    Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
                    foreach (Collider c in cols)
                    {
                        c.enabled = false;
                    }

                    heldBowl = bowl;

                    heldBowl.OnPutInMicrowave();
                }
                else if (bowl != null)
                {
                    //まだ温められない
                }
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
            if (dist < minDistance) 
            {
                minDistance = dist; closest = p; 
            }
        }
        return closest;
    }
}
