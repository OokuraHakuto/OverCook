using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour, IInteracttable
{
    [Header("設定")]
    public Transform holdPoint;
    private GameObject heldItem;
    [Header("制限設定")]
    public bool canPlaceItem = true;

    [Header("完成したアイスのプレファブ")]
    public GameObject cupVanillaPrefab;
    public GameObject cupChocolatePrefab;
    public GameObject cupStrawberryPrefab;
    public GameObject cupMatchaPrefab;

    [Header("ナビゲーション")]
    public GameObject arrow1P;  // 1p赤矢印
    public GameObject arrow2P;  // 2p青矢印

    void Start()
    {
        if (holdPoint.childCount > 0)
        {
            heldItem = holdPoint.GetChild(0).gameObject;
            Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
            foreach (Collider c in cols) c.enabled = false;
        }
    }

    // 更新
    void Update()
    {
        UpdateNavArrows();
    }

    // 矢印の更新
    void UpdateNavArrows()
    {
        // 初期化（一旦消す）
        if (arrow1P != null) arrow1P.SetActive(false);
        if (arrow2P != null) arrow2P.SetActive(false);

        // プレイヤーを検索
        var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            if (p.heldItem == null) continue; // 手ぶらなら用はない

            // ---------------------------------------------------------
            // ケースA：机が「空」で、プレイヤーが「冷凍ボウル」を持っている
            // ---------------------------------------------------------
            if (heldItem == null && canPlaceItem)
            {
                Bowl bowl = p.heldItem.GetComponent<Bowl>();

                // 「凍ってる」かつ「焦げてない」ボウルなら置かせたい
                if (bowl != null && bowl.isFrozen && !bowl.isBurnt)
                {
                    ShowArrow(p.playerID);
                }
            }
            // ---------------------------------------------------------
            // ケースB：机に「冷凍ボウル」があり、プレイヤーが「カップ」を持っている
            // ---------------------------------------------------------
            else if (heldItem != null)
            {
                // 机にあるアイテムがボウルかチェック
                Bowl tableBowl = heldItem.GetComponent<Bowl>();

                // プレイヤーがカップを持っているかチェック（Cupクラスがあると仮定）
                // ※もしCupクラスがないなら p.heldItem.name.Contains("Cup") 等で代用
                Cup playerCup = p.heldItem.GetComponent<Cup>();

                // 「机のが冷凍ボウル」＆「プレイヤーがカップ」＆「カップがまだ空」なら
                if (tableBowl != null && tableBowl.isFrozen && playerCup != null && !playerCup.isFull
                    && !tableBowl.isCracked && !tableBowl.isBurnt)
                {
                    ShowArrow(p.playerID);
                }
            }
        }
    }

    // 指定したプレイヤーIDの矢印をONにするヘルパー関数
    void ShowArrow(int playerID)
    {
        if (playerID == 1 && arrow1P != null) arrow1P.SetActive(true);
        if (playerID == 2 && arrow2P != null) arrow2P.SetActive(true);
    }

    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // パターンA：台に何かある
        if (heldItem != null)
        {
            // --- ケース1：手ぶら（拾う） ---
            if (player.heldItem == null)
            {
                Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
                foreach (Collider c in cols) c.enabled = true;

                GameObject itemToPass = heldItem;
                player.PickUpItem(itemToPass);

                // スケールを手持ち用に
                ItemSettings settings = itemToPass.GetComponent<ItemSettings>();
                if (settings != null)
                {
                    itemToPass.transform.localScale = settings.onPlayerScale;
                    itemToPass.transform.localPosition = settings.holdPositionOffset;
                }
                else
                {
                    itemToPass.transform.localScale = Vector3.one;
                }

                heldItem = null;
                //アイテムを取り出した
            }
            // --- ケース2：何か持っている（合体・投入・混ぜる） ---
            else
            {
                Bowl bowl = heldItem.GetComponent<Bowl>();
                Cup playerCup = player.heldItem.GetComponent<Cup>(); 
                Whisk whisk = player.heldItem.GetComponent<Whisk>();

                // 盛り付け（Plating）処理
                // 「机にボウル」があり、「プレイヤーがカップ」を持っている場合
                if (bowl != null && playerCup != null)
                {
                    // カップが空っぽ(false)で、ボウルからすくい取れたら
                    if (!playerCup.isFull && bowl.TryScoopIceCream())
                    {
                        // プレイヤーの手にある「空カップ」を消す
                        GameObject emptyCup = player.heldItem;
                        player.ReleaseItem();
                        Destroy(emptyCup);

                        // ボウルの味に応じて生成するプレファブを決定する
                        GameObject prefabToSpawn = null;

                        if (bowl.hasChocolate)
                        {
                            prefabToSpawn = cupChocolatePrefab;
                        }
                        else if (bowl.hasStrawberry)
                        {
                            prefabToSpawn = cupStrawberryPrefab;
                        }
                        else if (bowl.hasMatcha)
                        {
                            prefabToSpawn = cupMatchaPrefab;
                        }
                        else
                        {
                            prefabToSpawn = cupVanillaPrefab; // デフォルト
                        }

                        // 「完成アイス」を生成して持たせる
                        if (prefabToSpawn != null)
                        {
                            GameObject newIce = Instantiate(prefabToSpawn);
                            player.PickUpItem(newIce);

                            ItemSettings settings = newIce.GetComponent<ItemSettings>();
                            if (settings != null)
                            {
                                newIce.transform.localScale = settings.onPlayerScale;
                                newIce.transform.localPosition = settings.holdPositionOffset;
                                newIce.transform.localRotation = Quaternion.Euler(settings.onPlayerRotation);
                            }
                        }

                        return; // ここで処理終了
                    }
                }
                else if (whisk != null && bowl != null)                // 泡だて器で混ぜる
                {
                    if (bowl.IsReadyToMix())
                    {
                        bowl.AddMixProgress();

                        player.PlayMixAnimation();
                    }
                }
                // 食材を入れる
                else if (bowl != null)
                {
                    bool success = bowl.AddIngredient(player.heldItem);
                    if (success)
                    {
                        GameObject ingredient = player.heldItem;
                        player.ReleaseItem();
                        Destroy(ingredient);
                    }
                }
                else
                {
                    //手がふさがっている
                }
            }
        }
        // パターンB：台が空（置く）
        else
        {
            if (!canPlaceItem) return;

            if (player.heldItem != null)
            {
                GameObject itemToPlace = player.heldItem;
                player.ReleaseItem();

                itemToPlace.transform.SetParent(holdPoint);
                itemToPlace.transform.localPosition = Vector3.zero;
                itemToPlace.transform.localRotation = Quaternion.identity;

                // スケールを机用に
                ItemSettings settings = itemToPlace.GetComponent<ItemSettings>();
                if (settings != null) itemToPlace.transform.localScale = settings.onTableScale;
                else itemToPlace.transform.localScale = Vector3.one;

                Collider[] cols = itemToPlace.GetComponentsInChildren<Collider>();
                foreach (Collider c in cols) c.enabled = false;

                heldItem = itemToPlace;
                //台に置いた
            }
        }
    }

    // プレイヤー探索（共通）
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