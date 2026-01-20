using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    [System.Serializable]
    public class Menu
    {
        public string iceName;      // 正解のアイテム名（例: Cup_Banilla）※スペル注意
        public GameObject uiPrefab; // 表示するアイコンのプレハブ
    }

    //注文データ
    [System.Serializable]
    public class OrderData
    {
        public GameObject uiObject;
        public string iceName;
    }

    [Header("メニュー一覧（ここに全種類登録する）")]
    public List<Menu> menuList = new List<Menu>();

    [Header("注文UIを並べる親オブジェクト")]
    public Transform orderPanelParent;

    [Header("注文の最大数")]
    public int maxOrders = 5;

    // 現在の注文リスト
    private List<OrderData> currentOrders = new List<OrderData>();

    [Header("難易度ごとの注文間隔（秒）")]
    public Vector2 easyInterval = new Vector2(10f, 15f);
    public Vector2 normalInterval = new Vector2(7f, 10f);
    public Vector2 hardInterval = new Vector2(4f, 6f);

    private bool isOrdering = true; // 注文を出し続けるかどうかのフラグ

    [Header("ゲーム開始後の最初の待機時間（秒）")]
    public float startDelay = 1.5f;

    void Awake()
    {
        Time.timeScale = 1f;

        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        // ランダム間隔で注文を出すコルーチンを開始
        StartCoroutine(OrderLoopRoutine());
    }

    // 注文受付のループ
    IEnumerator OrderLoopRoutine()
    {
        // 最初のウェイト（ゲーム開始直後にドカッと来ないように）
        yield return new WaitForSeconds(startDelay);

        while (isOrdering)
        {
            // 注文を出す
            AddRandomOrder();

            // 次の注文までの待ち時間を決める
            float waitTime = GetIntervalByDifficulty();

            // 待つ
            yield return new WaitForSeconds(waitTime);
        }
    }

    // 難易度に応じたランダムな時間を返す
    float GetIntervalByDifficulty()
    {
        int diff = 1; // デフォルトはNormal
        if (SelectionManager.instance != null)
        {
            diff = SelectionManager.instance.difficulty;
        }

        Vector2 range;
        switch (diff)
        {
            case 0: range = easyInterval; break;   // Easy
            case 1: range = normalInterval; break; // Normal
            case 2: range = hardInterval; break;   // Hard
            default: range = normalInterval; break;
        }

        float result = Random.Range(range.x, range.y);

        return result;
    }

    // ランダムな注文を追加
    public void AddRandomOrder()
    {
        if (currentOrders.Count >= maxOrders)
        {
            return; // ここで処理を終わらせる
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.seChime);
        }

        // ランダムにメニューを選ぶ
        int randomIndex = Random.Range(0, menuList.Count);
        Menu selectedMenu = menuList[randomIndex];

        // UIを生成
        GameObject newIcon = Instantiate(selectedMenu.uiPrefab, orderPanelParent);

        // 出現アニメーション（ポヨン！）
        newIcon.transform.localScale = Vector3.zero;
        newIcon.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        // Listに追加（末尾に追加＝一番新しい）
        OrderData newData = new OrderData();
        newData.uiObject = newIcon;
        newData.iceName = selectedMenu.iceName;

        currentOrders.Add(newData);
    }

    // 納品チェック（DeliverySpotから呼ばれる）
    public bool TryDelivery(string deliveredItemName)
    {
        OrderData matchedOrder = null;

        // 今ある注文の中に、持ってきたアイテムと同じ名前のものがあるか？
        for (int lpc = 0; lpc < currentOrders.Count; lpc++)
        {
            // 名前が一致したら発見
            if (currentOrders[lpc].iceName == deliveredItemName)
            {
                matchedOrder = currentOrders[lpc];
                break; // 最初に見つかった＝一番古いものなので、これ以上探さなくてOK
            }
        }

        if (matchedOrder != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySE(AudioManager.Instance.seSuccess);
            }

            // 成功した注文を消す
            RemoveOrder(matchedOrder);

            return true; // 成功！
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySE(AudioManager.Instance.seFail);
            }

            return false; // 失敗...
        }
    }

    // 注文を消す処理
    void RemoveOrder(OrderData orderData)
    {
        // リストから消す
        currentOrders.Remove(orderData);

        // UIを消す
        if (orderData.uiObject != null)
        {
            orderData.uiObject.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
            {
                Destroy(orderData.uiObject);
            });
        }
    }

    // ゲーム終了時に呼ぶ注文ストップ関数
    public void StopOrdering()
    {
        isOrdering = false; // これでwhileループが止まる
        StopAllCoroutines(); // 待機中のコルーチンも強制停止
    }
}
