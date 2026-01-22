using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Animation_1P : MonoBehaviour
{
    public GameObject upArrow, downArrow, cursor;
    int diff = 0;

    public GameObject OKText, player;
    bool lotF;

    // Start is called before the first frame update
    void Start()
    {
        lotF = true;
    }

    // Update is called once per frame
    void Update()
    {
        Choose();
        Decide();
    }

    void Choose()
    {
        if (Input.GetKeyDown("w"))
        {
            upArrow.transform.DOMove(new Vector3(395f, 560f, 0f), 0.05f).SetLoops(2, LoopType.Yoyo);
        }
        if (Input.GetKeyDown("s"))
        {
            downArrow.transform.DOMove(new Vector3(395f, 72f, 0f), 0.05f).SetLoops(2, LoopType.Yoyo);
        }

        if (Input.GetKeyDown("a"))
        {
            if (diff == 1)
                cursor.transform.DOLocalMove(new Vector3(/*53*/268f, 0f, 0f), 0.1f).SetLoops(1, LoopType.Incremental);
            else if (diff == 2)
                cursor.transform.DOLocalMove(new Vector3(/*152*/368f, -312f, 0f), 0.1f).SetLoops(1, LoopType.Incremental);

            Debug.Log(diff);
            if (diff > 0)
                diff--;
        }
        if (Input.GetKeyDown("d"))
        {
            if (diff == 0)
                cursor.transform.DOLocalMove(new Vector3(367f, -312f, 0f), 0.1f).SetLoops(1, LoopType.Incremental);
            else if (diff == 1)
                cursor.transform.DOLocalMove(new Vector3(/*251*/468f, -624f, 0f), 0.1f).SetLoops(1, LoopType.Incremental);

            Debug.Log(diff);
            if (diff < 2)
                diff++;
        }
    }

    void Decide()
    {
        if (Go2GameMainManager.OKflg1P && lotF)
        {
            player.transform.DORotate(Vector3.up * 515f, 0.5f, RotateMode.FastBeyond360);
            OKText.transform.DOScale(new Vector3(2, 2, 2), 0.25f).SetLoops(2, LoopType.Yoyo);
            lotF = false;
        }
        else if (Go2GameMainManager.OKflg1P == false)
        {
            lotF = true;
        }
    }

}