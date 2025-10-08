using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CTRLer_2P : MonoBehaviour
{
    //SoundMgr soundMgr = new SoundMgr();

    public GameObject cursor;
    int diff;
    public int chara;

    //↓共有動作のスプリクトに移す？
    public GameObject penguin1, penguin2, penguin3, penguin4, penguin5,
                  penguin6, penguin7, penguin8, penguin9, nanchara;
    public Transform oya;
    Vector3 penguinSize = new Vector3(0.75f, 0.75f, 0.75f),
            nancharaSize = new Vector3(1.5f, 1.5f, 1.5f);


    // Start is called before the first frame update
    void Start()
    {
        chara = 2;
        diff = 0;

        DispChara(chara);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("あぷで2P");

        SelectChara();
        SelectDiff();
    }

    public void SelectChara()
    {
        if (Input.GetKeyDown("up"))
        {
            chara--;
            if (chara <= 0) chara = 10;

            DispChara(chara);
            //soundMgr.PlaySE(1);
        }
        if (Input.GetKeyDown("down"))
        {
            chara++;
            if (chara >= 11) chara = 1;

            DispChara(chara);
            //soundMgr.PlaySE(1);
        }
    }

    public void SelectDiff()
    {
        if (Input.GetKeyDown("left") && diff > 0)
        {
            cursor.transform.Translate(-10.6f, 33.5f, 0);
            //soundMgr.PlaySE(2);

            diff--;
            Debug.Log("←おうか " + diff);
            Debug.Log(diff);
        }

        if (Input.GetKeyDown("right") && diff < 2)
        {
            cursor.transform.Translate(10.6f, -33.5f, 0);
            //soundMgr.PlaySE(2);

            diff++;
            Debug.Log("→おうか " + diff);
        }
    }

    //↓共有動作のスプリクトに移す？
    void DispChara(int num)
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
}
