using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 

public class TitlePopIn : MonoBehaviour
{
    public Image titleImage; 

    void Start()
    {
        
        transform.localScale = Vector3.zero;
        titleImage.color = new Color(1, 1, 1, 0);

        //ポップインアニメーション
        transform.DOScale(Vector3.one, 2f).SetEase(Ease.OutBack);

        // フェードイン
        titleImage.DOFade(0.5f, 0.5f);
    }
}
