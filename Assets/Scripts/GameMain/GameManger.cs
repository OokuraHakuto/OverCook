using UnityEngine;
using TMPro; // TextMeshProを使うために必要

public class GameManager : MonoBehaviour
{
    [Header("時間設定 (秒)")]
    public float roundTime = 180f; // 1ラウンドの時間（例: 180秒 = 3分）

    [Header("UI関連")]
    public TextMeshProUGUI timerText; // 時間を表示するUIテキスト

    private float currentTime; // 残り時間

    void Start()
    {
        currentTime = roundTime;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime = 0;
            // ここに時間がゼロになった時の処理（ゲームオーバーなど）を書く
            Debug.Log("時間切れ！");
        }

        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}