using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrWave : MonoBehaviour, IInteracttable
{
    [Header("�n�����̂ɂ����鎞��")]
    public float meltTime = 5.0f;

    //�n�����Ă���r�����ǂ���
    private bool isMelting = false;

    public void Interact()
    {
        // �����n�������łȂ���΁A�n�����������J�n����
        if (!isMelting)
        {
            Debug.Log("�����W��Interact()���Ă΂�܂����I");
            StartMeltingProcess();
        }
        else
        {
            Debug.Log("���A�n�������ł��I");
        }
    }

    private void StartMeltingProcess()
    {

    }

    private void FinishMelting()
    {

    }

}
