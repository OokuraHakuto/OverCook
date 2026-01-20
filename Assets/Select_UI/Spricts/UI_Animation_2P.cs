using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Animation_2P : MonoBehaviour
{
    public GameObject upArrow, downArrow, cursor;
    int diff = 0;

    public GameObject OKText, player;
    bool lotF;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Choose();
        Decide();
    }


    void Choose()
    {
        if (Input.GetKeyDown("up"))
        {
            upArrow.transform.DOMove(new Vector3(700f, 560f, 0f), 0.05f).SetLoops(2, LoopType.Yoyo);
        }
        if (Input.GetKeyDown("down"))
        {
            downArrow.transform.DOMove(new Vector3(700f, 72f, 0f), 0.05f).SetLoops(2, LoopType.Yoyo);
        }
        /*
        if (Input.GetKeyDown("left"))
        {
            if (diff == 1)
                cursor.transform.DOLocalMove(new Vector3(484f, 0f, 0f), 0.1f).SetLoops(1, LoopType.Incremental);
            else if (diff == 2)
                cursor.transform.DOLocalMove(new Vector3(583f, -312f, 0f), 0.1f).SetLoops(1, LoopType.Incremental);

            Debug.Log(diff);
            if (diff > 0)
                diff--;
        }
        if (Input.GetKeyDown("right"))
        {
            if (diff == 0)
                cursor.transform.DOLocalMove(new Vector3(583f, -312f, 0f), 0.1f).SetLoops(1, LoopType.Incremental);
            else if (diff == 1)
                cursor.transform.DOLocalMove(new Vector3(682f, -624f, 0f), 0.1f).SetLoops(1, LoopType.Incremental);

            Debug.Log(diff);
            if (diff < 2)
                diff++;
        }
        */
    }

    void Decide()
    {
        if (Go2GameMainManager.OKflg2P && lotF)
        {
            player.transform.DORotate(Vector3.up * 529f, 0.5f, RotateMode.FastBeyond360);
            OKText.transform.DOScale(new Vector3(2, 2, 2), 0.25f).SetLoops(2, LoopType.Yoyo);
            lotF = false;
        }
        else if (Go2GameMainManager.OKflg2P == false)
        {
            lotF = true;
        }
    }
}