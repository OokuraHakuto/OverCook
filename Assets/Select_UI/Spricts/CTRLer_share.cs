using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CTRLer_share : MonoBehaviour
{
    public int playerNum;

    // (�ȗ�... backCnt, backSld �Ȃǂ̕ϐ��͂��̂܂�)
    public Slider backSld;
    public GameObject backSldGo;
    float backCnt;

    // (�ȗ�... okCnt, okSld �Ȃǂ̕ϐ��͂��̂܂�)
    float okCnt;
    public Slider okSld;
    public GameObject okSldGo, standbyTextGo;
    public Text standbyText;
    bool okFlg;

    //�����̃v���C���[�R���g���[���[�i1P�܂���2P�j��ێ�����ϐ� ������
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
        // (�ȗ�... ���̃R�[�h�̂܂�)
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
            if (okFlg == false) // ���̏u�ԂɈ�x�������s
            {
                if (playerNum == 1 && controller1P != null)
                {
                    // P1�R���g���[���[����I�񂾃v���t�@�u���擾���A�^�щ��ɓn��
                    SelectionManager.instance.player1Prefab = controller1P.GetCurrentSelectedPrefab();
                    Debug.Log("P1 �m��: " + SelectionManager.instance.player1Prefab.name);
                }
                else if (playerNum == 2 && controller2P != null)
                {
                    // P2�R���g���[���[����I�񂾃v���t�@�u���擾���A�^�щ��ɓn��
                    SelectionManager.instance.player2Prefab = controller2P.GetCurrentSelectedPrefab();
                    Debug.Log("P2 �m��: " + SelectionManager.instance.player2Prefab.name);
                }
                //soundMgr.PlaySE(3);
            }

            okFlg = true;
        }

        // (�ȉ��� standbyText �̏����͕ύX�Ȃ�)
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
