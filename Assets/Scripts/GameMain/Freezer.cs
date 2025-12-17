using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Freezer : MonoBehaviour, IInteracttable
{
    [Header("設定")]
    public Transform holdPoint;       // ボウルを置く場所
    public float freezeTime = 3.0f;   // 完成までの時間
    public GameObject doorObject;     // ドア（アニメーション用・今回は省略可）

    // 将来のUIゲージ用に public にしておきます
    [Header("状態確認（UI用）")]
    public float currentTimer = 0f;
    public bool isFreezing = false;

    private GameObject heldItem;
    private Bowl heldBowl; // 処理中のボウル

    void Update()
    {
        // 冷凍処理
        if (isFreezing && heldBowl != null)
        {
            currentTimer += Time.deltaTime;

            // 完了判定
            if (currentTimer >= freezeTime)
            {
                currentTimer = freezeTime;
                isFreezing = false;

                // ボウルを完成状態にする
                heldBowl.Freeze();
                Debug.Log("冷凍完了！");
            }
        }
    }

    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // パターンA：冷凍庫にボウルがある（取り出す）
        if (heldItem != null)
        {
            if (player.heldItem == null)
            {
                // まだ冷凍中なら取り出せないようにする？（今回はいつでも取れる仕様にします）
                if (isFreezing)
                {
                    Debug.Log("まだ冷凍中です...");
                    // 途中で取り出すとリセットするならここで行う
                    isFreezing = false;
                    currentTimer = 0f;
                }

                // プレイヤーに渡す
                player.PickUpItem(heldItem);

                // アイテム設定リセット（Counterと同じ）
                ItemSettings settings = heldItem.GetComponent<ItemSettings>();
                if (settings != null)
                {
                    heldItem.transform.localScale = settings.onPlayerScale;
                    heldItem.transform.localPosition = settings.holdPositionOffset;
                    heldItem.transform.localRotation = Quaternion.Euler(settings.onPlayerRotation);
                }

                heldItem = null;
                heldBowl = null;
            }
        }
        // パターンB：冷凍庫が空（入れる）
        else
        {
            if (player.heldItem != null)
            {
                Bowl bowl = player.heldItem.GetComponent<Bowl>();

                // ★条件チェック：ボウルを持っていて、かつ「冷凍できる状態」か？
                if (bowl != null && bowl.IsReadyToFreeze())
                {
                    // プレイヤーから受け取る
                    heldItem = player.heldItem;
                    player.ReleaseItem();

                    // 冷凍庫に配置
                    heldItem.transform.SetParent(holdPoint);
                    heldItem.transform.localPosition = Vector3.zero;
                    heldItem.transform.localRotation = Quaternion.identity;

                    // スケール調整
                    ItemSettings settings = heldItem.GetComponent<ItemSettings>();
                    if (settings != null) heldItem.transform.localScale = settings.onTableScale;

                    // ★冷凍開始！
                    heldBowl = bowl;
                    currentTimer = 0f;
                    isFreezing = true;
                    Debug.Log("冷凍を開始します...");
                }
                else
                {
                    Debug.Log("それは冷凍できません（まだ混ざっていないか、違うアイテムです）");
                }
            }
        }
    }

    // プレイヤー探索（共通）
    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 10.0f;
        foreach (var p in players) { float dist = Vector3.Distance(transform.position, p.transform.position); if (dist < minDistance) { minDistance = dist; closest = p; } }
        return closest;
    }
}
