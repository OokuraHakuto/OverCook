using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CTRLer_share : MonoBehaviour
{
    //SoundMgr soundMgr = new SoundMgr();

    public int playerNum;

    //[Header("３Ｄモデル")]
    public GameObject penguin1, penguin2, penguin3, penguin4, penguin5,
                      penguin6, penguin7, penguin8, penguin9, nanchara;
    //public Transform oya;
    Vector3 penguinSize = new Vector3(0.75f, 0.75f, 0.75f),
            nancharaSize = new Vector3(1.5f, 1.5f, 1.5f);

    //[Header("タイトルへ戻る系で使うやつ")]
    float backCnt;
    public Slider backSld;
    public GameObject backSldGo;

    //[Header("決定/待ち状態ので使うやつ")]
    float okCnt;
    public Slider okSld;
    public GameObject okSldGo, standbyTextGo;
    public Text standbyText;

    bool okFlg;

    private void Start()
    {
        okFlg = false;
    }

    private void Update()
    {
        BackTitle(playerNum);
        OKorWait(playerNum);
    }



    /*
    public void DispChara(int num)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        switch (num)
        {
            case 1:
                Instantiate(penguin1, oya);
                oya.transform.localScale = penguinSize;
                break;
            case 2:
                Instantiate(penguin2, oya);
                oya.transform.localScale = penguinSize;
                break;
            case 3:
                Instantiate(penguin3, oya);
                oya.transform.localScale = penguinSize;
                break;
            case 4:
                Instantiate(penguin4, oya);
                oya.transform.localScale = penguinSize;
                break;
            case 5:
                Instantiate(penguin5, oya);
                oya.transform.localScale = penguinSize;
                break;
            case 6:
                Instantiate(penguin6, oya);
                oya.transform.localScale = penguinSize;
                break;
            case 7:
                Instantiate(penguin7, oya);
                oya.transform.localScale = penguinSize;
                break;
            case 8:
                Instantiate(penguin8, oya);
                oya.transform.localScale = penguinSize;
                break;
            case 9:
                Instantiate(penguin9, oya);
                oya.transform.localScale = penguinSize;
                break;
            case 10:
                Instantiate(nanchara, oya);
                oya.transform.localScale = nancharaSize;
                break;
        }
    }
    */



    public void BackTitle(int num)
    {
        switch (num)
        {
            case 1:
                if (Input.GetKey("q"))
                {
                    Debug.Log("Qおうか、タイトルへ");

                    backSldGo.SetActive(true);
                    backCnt += Time.deltaTime;

                    if (okFlg)
                    {
                        okFlg = false;
                        //soundMgr.PlaySE(4);
                    }
                }
                else
                {
                    backCnt = 0;
                    backSldGo.SetActive(false);
                }
                break;

            case 2:
                if (Input.GetKey("l"))
                {
                    Debug.Log("Lおうか、タイトルへ");

                    backSldGo.SetActive(true);
                    backCnt += Time.deltaTime;

                    if (okFlg)
                    {
                        okFlg = false;
                        //soundMgr.PlaySE(4);
                    }
                }
                else
                {
                    backCnt = 0;
                    backSldGo.SetActive(false);
                }
                break;
        }

        backSld.value = backCnt;

        if (backCnt >= 1)
        {
            SceneManager.LoadScene("Title");
        }
    }


    public void OKorWait(int num)
    {
        switch (num)
        {
            case 1:
                if (Input.GetKey("e") && okFlg == false)
                {
                    Debug.Log("Eおうか");

                    okCnt += Time.deltaTime;
                    okSldGo.SetActive(true);
                }
                else
                {
                    okCnt = 0;
                    okSldGo.SetActive(false);
                }

                Go2GameMainManager.OKflg1P = okFlg;
                break;

            case 2:
                if (Input.GetKey("p") && okFlg == false)
                {
                    Debug.Log("Pおうか");

                    okCnt += Time.deltaTime;
                    okSldGo.SetActive(true);
                }
                else
                {
                    okCnt = 0;
                    okSldGo.SetActive(false);
                }

                Go2GameMainManager.OKflg2P = okFlg;
                break;
        }

        okSld.value = okCnt;

        if (okCnt >= 1)
        {
            okFlg = true;
            //soundMgr.PlaySE(3);
        }

        if (okFlg)
        {
            standbyText.text = "OK!";
            standbyText.color = Color.yellow;
            
        }
        else
        {
            standbyText.text = "Wait...";
            standbyText.color = Color.cyan;
        }
    }
}
                                                                 