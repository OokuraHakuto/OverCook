using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Freezer : MonoBehaviour, IInteracttable
{
    [Header("冷やすのにかかる時間")]
    public float freezeTime = 5.0f;

    private bool isFreezing = false;


    //インタラクト処理
    public void Interact()
    {
        //まだ冷やしてなければ開始
        if(!isFreezing)
        {
            Debug.Log("冷凍庫に入れました");

            StartFreezing();
        }
        else
        {
            Debug.Log("今冷やし中です");
        }
    }

    private void StartFreezing()
    {
        isFreezing = true;

        Invoke("FinishFreezing", freezeTime);
    }

    private void FinishFreezing()
    {
        isFreezing = false;

        Debug.Log("アイスが固まりました！");

        //アイスを取り出し可能にする処理などを追加する
    }
}
