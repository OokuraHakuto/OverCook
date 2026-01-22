using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Go2GameMainManager : MonoBehaviour
{
    public static bool OKflg1P, OKflg2P;

    // Update is called once per frame
    void Update()
    {
        if(OKflg1P && OKflg2P)
        {
            SceneManager.LoadScene("GameMain");
        }
    }
}