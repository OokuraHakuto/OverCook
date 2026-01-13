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

    public int currentScore = 0; // 現在のスコア
    private float currentTime; // 残り時間
    public bool isPlaying = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // シーン遷移で消えないように
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 新しく作られた場合は削除
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーン読み込み完了時に実行される関数
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 例: "GameMain" シーンの時だけ初期化する
        if (scene.name == "GameMain")
        {
            // 1. UIの再取得（前のシーンのUIは消えているため、新しく探す）
            GameObject timerObj = GameObject.Find("TimerText"); // ※Hierarchyの名前
            if (timerObj) timerText = timerObj.GetComponent<TextMeshProUGUI>();

            GameObject scoreObj = GameObject.Find("ScoreText"); // ※Hierarchyの名前
            if (scoreObj) scoreText = scoreObj.GetComponent<TextMeshProUGUI>();

            GameObject centerObj = GameObject.Find("CenterText"); // ※Hierarchyの名前
            if (centerObj) centerText = centerObj.GetComponent<TextMeshProUGUI>();

            // 2. ゲーム開始処理を呼ぶ
            GameStart();
        }
    }

    void Start()
    {
        currentTime = roundTime;
        UpdateScoreDisplay();

        StartCoroutine(CountDownRoutine());
    }

    // 初期化関数
    public void GameStart()
    {
        StopAllCoroutines(); // 前のカウントダウンなどが残っていたら止める

        currentTime = roundTime;
        currentScore = 0; // スコアもリセット（必要なら）
        isPlaying = false; // いったん停止

        UpdateScoreDisplay();
        UpdateTimerDisplay();

        // カウントダウン開始
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
        if(timerText!=null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
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
        isPlaying = true; // ここからゲーム開始！

        yield return new WaitForSeconds(1f);
        if (centerText != null) centerText.text = ""; // テキストを消す
    }

    // ゲーム終了処理
    void GameFinish()
    {
        isPlaying = false; // 時間を止める

        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.StopOrdering();
        }

        if (centerText != null)
        {
            centerText.text = "FINISH!";
        }

        Debug.Log("ゲーム終了！");

        // もし3秒後にリザルト画面（別シーン）に飛ばすなら、下の行の // を外す
        Invoke("GoToResultScene", 3f);
    }

    void GoToResultScene()
    {
        SceneManager.LoadScene("Result"); // シーン名を指定
    }
}