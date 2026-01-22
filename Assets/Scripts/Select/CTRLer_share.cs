using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CTRLer_share : MonoBehaviour
{
    public int playerNum;

    // (省略... backCnt, backSld などの変数はそのまま)
    public Slider backSld;
    public GameObject backSldGo;
    float backCnt;

    // (省略... okCnt, okSld などの変数はそのまま)
    float okCnt;
    public Slider okSld;
    public GameObject okSldGo, standbyTextGo;
    public Text standbyText;
    bool okFlg;

    //自分のプレイヤーコントローラー（1Pまたは2P）を保持する変数 ▼▼▼
    private CTRLer_1P controller1P;
    private CTRLer_2P controller2P;

    private void Start()
    {
        okFlg = false;

        if (playerNum == 1)
        {
            controller1P = GetComponent<CTRLer_1P>();
        }
        else if (playerNum == 2)
        {
            controller2P = GetComponent<CTRLer_2P>();
        }
    }

    private void Update()
    {
        BackTitle(playerNum);
        OKorWait(playerNum);
    }

    public void BackTitle(int num)
    {
        // (省略... 元のコードのまま)

        if (Input.GetKey("escape"))
        {
            backSld.value += Time.deltaTime;
            backSldGo.SetActive(true);
        }
        else
        {
            backSld.value = 0;
            backSldGo.SetActive(false);
        }

        if (backSld.value >= 1) SceneManager.LoadScene("Title");
    }

    public void OKorWait(int num)
    {
        switch (num)
        {
            case 1:
                if (Input.GetKey("e") && okFlg == false)
                {
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
            if (okFlg == false) // この瞬間に一度だけ実行
            {
                if (playerNum == 1 && controller1P != null)
                {
                    // P1コントローラーから選んだプレファブを取得し、運び屋に渡す
                    SelectionManager.instance.player1Prefab = controller1P.GetCurrentSelectedPrefab();
                    Debug.Log("P1 確定: " + SelectionManager.instance.player1Prefab.name);
                }
                else if (playerNum == 2 && controller2P != null)
                {
                    // P2コントローラーから選んだプレファブを取得し、運び屋に渡す
                    SelectionManager.instance.player2Prefab = controller2P.GetCurrentSelectedPrefab();
                    Debug.Log("P2 確定: " + SelectionManager.instance.player2Prefab.name);
                }
                //soundMgr.PlaySE(3);
            }

            okFlg = true;
        }

        // (以下の standbyText の処理は変更なし)
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

        if (okFlg)
        {
            switch (num)
            {
                case 1:
                    if (Input.GetKey("q"))
                    {
                        okFlg = false;
                    }
                    break;

                case 2:
                    if (Input.GetKey("l"))
                    {
                        okFlg = false;
                    }
                    break;
            }
        }
    }
}