using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Slider scoreSlider;
    public Image star1, star2, star3, anmaku;
    bool twinkF1, twinkF2, twinkF3;
    public Sprite twink, blank;
    public Text scoreText;
    public AudioSource AS;
    public AudioClip hoshi1, hoshi2, hoshi3;

    [Header("アニメーション時間")]
    public float countDuration = 2.0f;  // 2秒かけてカウントアップ

    // テキスト表示用に内部でカウントアップする変数
    private float displayScore = 0f;

    void Start()
    {
        ReSet();

        int targetScore = 0;

        if(GameManager.Instance != null )
        {
            targetScore = GameManager.Instance.currentScore;

            // リザルト中はゲームプレイフラグをoff
            GameManager.Instance.isPlaying = false;
        }

        //スライダー最大値
        scoreSlider.maxValue = 1500;

        //スライダーアニメーション
        scoreSlider.DOValue(targetScore, countDuration).SetEase(Ease.OutCubic);

        // キャラ操作を封じる
        DisablePlayerControl();
    }

    void Update()
    {
        if (scoreSlider.value >= 500 && twinkF1 == false)
        {
            AS.PlayOneShot(hoshi1);
            star1.sprite = twink;
            star1.transform.DOPunchScale(new Vector3(-1f, -1f, -1f), 1f);
            twinkF1 = true;
        }

        if (scoreSlider.value >= 1000 && twinkF2 == false)
        {
            AS.PlayOneShot(hoshi2);
            star2.sprite = twink;
            star2.transform.DOPunchScale(new Vector3(-1f, -1f, -1f), 1f);
            twinkF2 = true;
        }

        if (scoreSlider.value >= 1500 && twinkF3 == false)
        {
            AS.PlayOneShot(hoshi3);
            star3.sprite = twink;
            star3.transform.DOPunchScale(new Vector3(-1f, -1f, -1f), 1f);
            twinkF3 = true;
        }

        scoreText.text = displayScore.ToString("F0");

        if (Input.GetKeyDown("r")) ReSet();
    }

    void ReSet()
    {
        scoreSlider.value = 0;
        displayScore = 0;
        star1.sprite = blank;
        star2.sprite = blank;
        star3.sprite = blank;
        twinkF1 = false;
        twinkF2 = false;
        twinkF3 = false;
    }

    // キャラクター操作を向こうにする関数
    void DisablePlayerControl()
    {
        // シーン上のすべてのPlayerControllerを探す
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        foreach (var player in players)
        {
            // スクリプト自体をOFFにする
            player.enabled = false;

            // もし走りモーションのままなら止める
            Animator anim = player.GetComponent<Animator>();
            if (anim != null) anim.SetFloat("Speed", 0);

            // 物理挙動も止める
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.velocity = Vector3.zero;
        }
    }

    // リトライボタン等から呼ぶ
    public void OnRetry()
    {
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }

        SceneManager.LoadScene("GameMain"); 
    }
}