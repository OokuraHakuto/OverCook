using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitlePopIn : MonoBehaviour
{
    public Image titleImage;
    public float delayTime = 1.0f; 

    void Start()
    {
        // 初期状態
        transform.localScale = Vector3.zero;
        titleImage.color = new Color(1, 1, 1, 0); 

     

        // フェードイン
        titleImage.DOFade(1f, 1f)
            .SetDelay(delayTime); // 1秒待ってから開始

        // ポップイン
        transform.DOScale(Vector3.one, 1f)
            .SetEase(Ease.OutBack)
            .SetDelay(delayTime); // 1秒待ってから開始
    }
}