using UnityEngine;
using TMPro; // TextMeshProを使うために必要
using System.Collections;
using UnityEngine.SceneManagement; // シーン移動用

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("時間設定 (秒)")]
    public float roundTime = 180f; // 1ラウンドの時間（例: 180秒 = 3分）

    [Header("UI関連")]
    public TextMeshProUGUI timerText; // 時間を表示するUIテキスト
    public TextMeshProUGUI scoreText; // スコア用

    [Header("真ん中のテキスト")]
    public TextMeshProUGUI centerText;

    private int currentScore = 0; // 現在のスコア
    private float currentTime; // 残り時間
    public bool isPlaying = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentTime = roundTime;
        UpdateScoreDisplay();

        StartCoroutine(CountDownRoutine());
    }

    void Update()
    {
        if(isPlaying)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                currentTime = 0;
                GameFinish();
            }

            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // スコアを加算する関数（DeliverySpotから呼ばれる）
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreDisplay();
        Debug.Log("現在のスコア: " + currentScore);
    }

    // スコア表示を更新
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            // "0000" の形式で表示（例：100点なら "0100" になる）
            scoreText.text = currentScore.ToString("0000");
        }
    }

    IEnumerator CountDownRoutine()
    {
        isPlaying = false; // まだ動けない

        // 3...
        if (centerText != null) centerText.text = "3";
        yield return new WaitForSeconds(1f);

        // 2...
        if (centerText != null) centerText.text = "2";
        yield return new WaitForSeconds(1f);

        // 1...
        if (centerText != null) centerText.text = "1";
        yield return new WaitForSeconds(1f);

        // GO!
        if (centerText != null) centerText.text = "GO!";
        isPlaying = true; // ★ここからゲーム開始！

        yield return new WaitForSeconds(1f);
        if (centerText != null) centerText.text = ""; // テキストを消す
    }

    // ゲーム終了処理
    void GameFinish()
    {
        isPlaying = false; // 時間を止める

        if (centerText != null)
        {
            centerText.text = "FINISH!";
        }

        Debug.Log("ゲーム終了！");

        // ★もし3秒後にリザルト画面（別シーン）に飛ばすなら、下の行の // を外す
        // Invoke("GoToResultScene", 3f);
    }

    void GoToResultScene()
    {
        // SceneManager.LoadScene("ResultScene"); // シーン名を指定
    }
}