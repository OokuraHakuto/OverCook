using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrWave : MonoBehaviour, IInteracttable
{
    [Header("溶かすのにかかる時間")]
    public float meltTime = 5.0f;

    //溶かしている途中かどうか
    private bool isMelting = false;

    public void Interact()
    {
        // もし溶かし中でなければ、溶かす処理を開始する
        if (!isMelting)
        {
            Debug.Log("レンジのInteract()が呼ばれました！");
            StartMeltingProcess();
        }
        else
        {
            Debug.Log("今、溶かし中です！");
        }
    }

    private void StartMeltingProcess()
    {
        isMelting = true;
        Debug.Log("材料を溶かし始めます...(" + meltTime + "秒)");

        //ここに材料を持っていないときの処理やゲージなどの処理を記述予定

        Invoke("FinishMelting", meltTime);
    }

    private void FinishMelting()
    {
        isMelting = false;

        Debug.Log("材料が解けました");
    }

}
