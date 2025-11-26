using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        ReSet();
    }

    void Update()
    {
        if (scoreSlider.value >= 500 && twinkF1 == false)
        {
            star1.sprite = twink;
            star1.transform.DOPunchScale(new Vector3(-1f, -1f, -1f), 1f);
            twinkF1 = true;
        }

        if (scoreSlider.value >= 1000 && twinkF2 == false)
        {
            star2.sprite = twink;
            star2.transform.DOPunchScale(new Vector3(-1f, -1f, -1f), 1f);
            twinkF2 = true;
        }

        if (scoreSlider.value >= 1500 && twinkF3 == false)
        {
            star3.sprite = twink;
            star3.transform.DOPunchScale(new Vector3(-1f, -1f, -1f), 1f);
            twinkF3 = true;
        }

        scoreText.text = scoreSlider.value.ToString();
        scoreSlider.value += 17;

        if (Input.GetKeyDown("r")) ReSet();
    }

    void ReSet()
    {
        scoreSlider.value = 0;
        star1.sprite = blank;
        star2.sprite = blank;
        star3.sprite = blank;
        twinkF1 = false;
        twinkF2 = false;
        twinkF3 = false;
    }
}