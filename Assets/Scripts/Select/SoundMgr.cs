using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMgr : MonoBehaviour
{
    public AudioSource SESource;
    public AudioClip selectChara, selectDiff, OK, cancel;

    float cnt = 0;
    bool flg = false;

    // Start is called before the first frame update
    void Start()
    {
        //SESource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("w") || Input.GetKeyDown("s") || Input.GetKeyDown("up") || Input.GetKeyDown("down"))
        {
            PlaySE(1);
        }
        if (Input.GetKeyDown("a") || Input.GetKeyDown("d") || Input.GetKeyDown("left") || Input.GetKeyDown("right"))
        {
            PlaySE(2);
        }

        if(Input.GetKey("e")|| Input.GetKey("p"))
        {
            cnt++;

            if (cnt == 60)
            {
                flg = true;
                PlaySE(3);
            }
        }
        else
        {
            cnt = 0;
        }

        if(flg&&(Input.GetKeyDown("q") || Input.GetKeyDown("l")))
        {
            flg = false;
            PlaySE(4);
        }
    }

    public void PlaySE(int num)
    {
        switch (num)
        {
            case 1:
                SESource.PlayOneShot(selectChara);
                break;
            case 2:
                SESource.PlayOneShot(selectDiff);
                break;
            case 3:
                SESource.PlayOneShot(OK);
                break;
            case 4:
                SESource.PlayOneShot(cancel);
                break;
        }
    }
}
