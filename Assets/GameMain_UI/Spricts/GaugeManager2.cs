using DG.Tweening;
using System;
using UniGLTF.Extensions.VRMC_vrm;
using UnityEngine;
using UnityEngine.UI;

public class GaugeManager2 : MonoBehaviour
{
    public GameObject sliderObj;
    public Slider slider1;
    public Image fillArea;
    public float time;
    public Transform camera;

    Color col;

    void Start()
    {
        time = 0;
        slider1.value = 0;

        col = new Color(0, 1, 0);
        fillArea.color = col;
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time <= 5)
        {
            slider1.value = time;
        }
        else if (time <= 7.5f)
        {
            col.r = 1 * ((time-5) / 2.5f);
            fillArea.color = col;
        }
        else if (time <= 10)
        {
            col.g = 1 * ((10-time) / 2.5f);
            fillArea.color = col;
        }
        else
        {
            if (time % 0.5f <= 0.25f)
            {
                col.r -= 0.02f;
                col.g -= 0.02f;
                fillArea.color = col;
            }
            else
            {
                col.r += 0.02f;
                col.g += 0.02f;
                fillArea.color = col;
            }
        }

        if (Input.GetKey(KeyCode.R)) Start();

        /*
        slider1Obj.transform.LookAt(camera);
        slider2Obj.transform.LookAt(camera);
        */
    }
}
