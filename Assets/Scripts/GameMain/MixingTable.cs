using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingStation : MonoBehaviour, IInteracttable
{
    [Header("ボウルがハマる位置")]
    public Transform holdPoint;

    private GameObject heldItem; // 今乗っているボウル（GameObject）
    private Bowl heldBowl;       // そのボウルのスクリプト

    [Header("ナビゲーション")]
    public GameObject arrow1P;  // 1p赤矢印
    public GameObject arrow2P;  // 2p青矢印

    //更新
    void Update()
    {
        // 矢印更新
        UpdateNavArrows();
    }

    //矢印更新
    void UpdateNavArrows()
    {
        // 1初期化
        if (arrow1P != null) arrow1P.SetActive(false);
        if (arrow2P != null) arrow2P.SetActive(false);

        // ミキサーが使用中なら出さない（既にボウルが置いてあるなら）
        if (heldBowl != null) return; 

        // プレイヤー検索
        var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            if (p.heldItem == null) continue;
            Bowl bowl = p.heldItem.GetComponent<Bowl>();
            if (bowl == null) continue;

            // 判定：「混ぜる必要がある？」
            if (bowl.NeedsMixing())
            {
                if (p.playerID == 1 && arrow1P != null) arrow1P.SetActive(true);
                if (p.playerID == 2 && arrow2P != null) arrow2P.SetActive(true);
            }
        }
    }

    // プレイヤーが調べた時
    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // ---------------------------------------------------
        // パターンA：すでにボウルが乗っている
        // ---------------------------------------------------
        if (heldBowl != null)
        {
            // まだ混ざりきっていないなら → 「混ぜる！」
            if (heldBowl.IsReadyToMix())
            {
                heldBowl.AddMixProgress(); // ボウル側の混ぜる処理（音もここで鳴る）

                // プレイヤーに混ぜるモーションさせる
                player.PlayMixAnimation();

                return; // ここで終了（拾わない）
            }

            // もう混ざり終わってる（または混ぜられない）なら → 「プレイヤーが拾う」
            if (player.heldItem == null)
            {
                // ボウルをプレイヤーに渡す
                player.PickUpItem(heldItem);

                // 渡したときの音
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySE(AudioManager.Instance.sePlace);
                }

                // テーブル側の情報は消す
                heldItem = null;
                heldBowl = null;
            }
        }
        // ---------------------------------------------------
        // パターンB：ミキサーが空っぽ
        // ---------------------------------------------------
        else
        {
            // プレイヤーがボウルを持っていたら → 「置く」
            if (player.heldItem != null)
            {
                Bowl bowl = player.heldItem.GetComponent<Bowl>();

                // 「混ぜられる状態のボウル（溶けてる）」だけ置けるようにする？
                if (bowl != null && bowl.IsReadyToMix())
                {
                    // 置く音
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlaySE(AudioManager.Instance.sePlace);

                    heldItem = player.heldItem;
                    player.ReleaseItem(); // プレイヤーの手から離す

                    // ミキサーの位置にセット
                    heldItem.transform.SetParent(holdPoint);
                    heldItem.transform.localPosition = Vector3.zero;
                    heldItem.transform.localRotation = Quaternion.identity;

                    // 物理演算オフ（固定）
                    Rigidbody rb = heldItem.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.useGravity = false;
                    }

                    // コライダーをオン（Rayが当たるようにするため）
                    Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
                    foreach (Collider c in cols)
                    {
                        c.enabled = false;
                    }

                    heldBowl = bowl;
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
            if (dist < minDistance) { minDistance = dist; closest = p; }
        }
        return closest;
    }
}