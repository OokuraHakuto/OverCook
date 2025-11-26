using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Cursor : MonoBehaviour
{
    public GameObject cursor;
    int place = 1;

    public Button oneMore, go2Title;
    public Image anmaku;
    bool idou;
    int siin;
    public float anten = 0;

    void Update()
    {
        if (place == 1)
        {
            if (Input.GetKeyDown("d"))
            {
                cursor.transform.DOLocalMove(new Vector3(6.5f, -2.89f, -1f), 0.1f).SetLoops(1, LoopType.Incremental);
                //go2Title.
                place = 2;
            }
            if (Input.GetKey("e"))
            {
                siin = 1;
                idou = true;
            }
        }
        else if (place == 2)
        {
            if (Input.GetKeyDown("a"))
            {
                cursor.transform.DOLocalMove(new Vector3(1.49f, -2.89f, -1f), 0.1f).SetLoops(1, LoopType.Incremental);
                place = 1;
            }
            if (Input.GetKey("e"))
            {
                siin = 2;
                idou = true;
            }
        }

        if (idou)
        {
            Go2Scene(siin);
        }
    }

    public void Go2Scene(int num)
    {
        Debug.Log("aicfvasdnplod");


        if (anten < 1)
        {
            Color kura = anmaku.color;
            anten += Time.deltaTime;

            kura.a = anten;
            anmaku.color = kura;
        }
        else
        {
            switch (num)
            {
                case 1:
                    SceneManager.LoadScene("GameMain");
                    break;
                case 2:
                    SceneManager.LoadScene("Title");
                    break;
            }
        }
    }
}