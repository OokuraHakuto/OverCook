using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("音源ソース")]
    public AudioSource bgmSource; // BGM用（Loopオンにしておく）
    public AudioSource seSource;  // SE用

    [Header("オーディオクリップ登録")]
    public AudioClip bgmMain;     // メインBGM

    [Header("SEリスト")]
    public AudioClip seCursor;    // カーソル（決定・キャンセル兼用）
    public AudioClip seWhistle;   // ホイッスル
    public AudioClip seDrop;      // 食材ポチョン
    public AudioClip seMix;       // 混ぜる
    public AudioClip seRange;     // レンジ（チン！）
    public AudioClip seChime;     // 注文チャイム
    public AudioClip seSuccess;   // 納品成功（チャリーン）
    public AudioClip seFail;      // 納品失敗
    public AudioClip sePlace;     // 置く・拾う
    public AudioClip seThrow;     // 投げる

    void Awake()
    {
        // シングルトン化（どこからでも呼べるようにする）
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移しても消さない
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    // BGM再生
    public void PlayBGM()
    {
        if (bgmMain != null && bgmSource != null)
        {
            // 強制的にクリップセットして再生します
            bgmSource.clip = bgmMain;
            bgmSource.Play();
        }
    }

    // BGMを止める
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    // 普通のSEを鳴らす用
    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;
        seSource.pitch = 1.0f; // ピッチを戻す
        seSource.PlayOneShot(clip);
    }

    // UI専用（ピッチを変える）
    // type: 0=移動, 1=決定, 2=キャンセル
    public void PlayUISound(int type)
    {
        if (seCursor == null) return;

        seSource.pitch = 1.0f; // デフォルト

        switch (type)
        {
            case 0: // カーソル移動
                seSource.pitch = 1.0f;
                break;
            case 1: // 決定（高くする）
                seSource.pitch = 1.5f;
                break;
            case 2: // キャンセル（低くする）
                seSource.pitch = 0.8f;
                break;
        }
        seSource.PlayOneShot(seCursor);

        // 次に普通のSEを鳴らす時にピッチが戻るようにPlaySE側で1.0fに戻してます
    }
}
