using DG.Tweening;
using System;
using UniGLTF.Extensions.VRMC_vrm;
using UnityEngine;
using UnityEngine.UI;

public class GaugeManager1 : MonoBehaviour
{
    public GameObject slider1Obj, slider2Obj;
    public Slider slider1, slider2;
    public Image fillArea2;
    public float time;
    public Transform camera;

    Color col;

    void Start()
    {
        time = 0;
        slider1.value = 0;
        slider2.value = 5;

        col = fillArea2.color;

    }

    void Update()
    {
        time += Time.deltaTime;

        if (time <= 5)
        {
            slider1.value = time;
        }
        else if (time <= 10)
        {
            slider2.value = time;
        }
        else
        {
            if (time % 0.5f <= 0.25f)
            {
                col.r -= 0.02f;
                col.g -= 0.02f;
                fillArea2.color = col;
            }
            else
            {
                col.r += 0.02f;
                col.g += 0.02f;
                fillArea2.color = col;
            }
        }

        if (Input.GetKey(KeyCode.R)) Start();

        /*
        slider1Obj.transform.LookAt(camera);
        slider2Obj.transform.LookAt(camera);
        */
    }
}
