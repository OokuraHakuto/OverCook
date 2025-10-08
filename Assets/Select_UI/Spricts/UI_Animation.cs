using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Animation : MonoBehaviour
{
    public GameObject upArrow, downArrow, cursor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //1P
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
            cursor.transform.DOMoveX(-20f, 0.2f);
            cursor.transform.DOMoveY(20f, 0.2f);
        }
        if (Input.GetKeyDown("d"))
        {
            cursor.transform.DOMoveX(20f, 0.2f);
            cursor.transform.DOMoveY(-20f, 0.2f);
        }
    }
}
