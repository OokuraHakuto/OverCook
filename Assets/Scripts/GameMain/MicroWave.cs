using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrWave : MonoBehaviour, IInteracttable
{
    [Header("設定")]
    public float cookTime = 5.0f; // 完成までにかかる時間
    public float burnTime = 5.0f; // 完成してから焦げるまでの猶予時間
    public Transform holdPoint;   // 見た目的にボウルを置く場所

    private Bowl heldBowl;        // 今中に入っているボウル
    private float timer = 0f;     // タイマー

    // レンジの状態定義
    private enum State { Empty, Cooking, Completed, Burnt }
    private State currentState = State.Empty;

    void Update()
    {
        // ボウルが入っている時だけ時間を進める
        if (currentState != State.Empty && heldBowl != null)
        {
            timer += Time.deltaTime; // 時間を経過させる

            // 1. 調理中 → 完成
            if (currentState == State.Cooking)
            {
                if (timer >= cookTime)
                {
                    currentState = State.Completed;
                    Debug.Log("チン！完成！（早く取り出さないと焦げるぞ...）");
                    // ここでUIを「✅」に変える

                    heldBowl.Cook();
                }
                else
                {
                    // ここでUIのゲージを進める (timer / cookTime)
                }
            }
            // 2. 完成 → 焦げ
            else if (currentState == State.Completed)
            {
                // 完成してからの経過時間を計算
                float overTime = timer - cookTime;

                if (overTime >= burnTime)
                {
                    currentState = State.Burnt;
                    Debug.Log("うわああ！焦げた！！！");
                    // ここでUIを「🔥」に変える

                    heldBowl.Burn();
                }
                else
                {
                    // ここで「焦げるぞ！」という警告エフェクト（点滅など）
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
        if (heldBowl != null)
        {
            if (player.heldItem == null) // 手ぶらなら
            {
                // ★取り出す瞬間に状態を確定させる！
                if (currentState == State.Completed)
                {
                    heldBowl.Cook(); // 成功！
                }
                else if (currentState == State.Burnt)
                {
                    heldBowl.Burn(); // 失敗（焦げ）！
                }
                else if (currentState == State.Cooking)
                {
                    Debug.Log("まだ調理中です！取り出せません！");
                    return; // 調理中は取り出せない仕様にするならここでreturn
                }

                // プレイヤーに渡す
                player.PickUpItem(heldBowl.gameObject);

                // リセット
                heldBowl = null;
                currentState = State.Empty;
                timer = 0f;
                // UIを非表示にする
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

                if (bowl != null && bowl.IsReadyToCook())
                {
                    player.ReleaseItem(); // 手放す

                    // レンジの中に移動
                    bowl.transform.SetParent(holdPoint);
                    bowl.transform.localPosition = Vector3.zero;
                    bowl.transform.localRotation = Quaternion.identity;

                    bowl.transform.localScale = Vector3.one;

                    // ▼▼▼ 追加：コライダーを復活させる！ ▼▼▼
                    Collider[] cols = bowl.GetComponentsInChildren<Collider>();
                    foreach (Collider c in cols)
                    {
                        c.enabled = true;
                    }
                    // ▲▲▲ ここまで ▲▲▲

                    heldBowl = bowl;
                    currentState = State.Cooking;
                    timer = 0f;
                    Debug.Log("🌀 レンジ加熱スタート！");
                }
                else if (bowl != null)
                {
                    Debug.Log("そのボウルはまだ温められません");
                }
            }
        }
    }

    // プレイヤー探索（いつものやつ）
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
