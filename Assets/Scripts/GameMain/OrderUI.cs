using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    [Header("UI参照")]
    public Slider timeSlider;   // 自分のスライダー

    // 内部データ
    private float timeLimit;        // 時間制限
    private float currentTime;      // 現在時間
    private OrderManager manager;   // オーダーマネージャー

    // この注文が何アイスか
    public string iceName;

    // セットアップ関数（生成時にManagerから呼ばれる）
    public void Setup(OrderManager mgr, string name, float limit)
    {
        manager = mgr;
        iceName = name;
        timeLimit = limit;
        currentTime = limit;

        if (timeSlider != null)
        {
            timeSlider.maxValue = timeLimit;
            timeSlider.value = currentTime;
        }
    }

    // 更新
    void Update()
    {
        // 時間を減らす
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (timeSlider != null)
            {
                timeSlider.value = currentTime;
            }
        }
        else
        {
            // 時間切れ！
            OnTimeUp();
        }
    }

    // 時間切れ
    void OnTimeUp()
    {
        // 重複実行防止
        currentTime = 0;
        this.enabled = false;

        // マネージャーに報告
        if (manager != null)
        {
            manager.OnOrderTimeUp(this);
        }
    }
}
