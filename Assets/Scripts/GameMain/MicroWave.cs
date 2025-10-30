using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrWave : MonoBehaviour, IInteracttable
{
    [Header("—n‚©‚·‚Ì‚É‚©‚©‚éŠÔ")]
    public float meltTime = 5.0f;

    //—n‚©‚µ‚Ä‚¢‚é“r’†‚©‚Ç‚¤‚©
    private bool isMelting = false;

    public void Interact()
    {
        // ‚à‚µ—n‚©‚µ’†‚Å‚È‚¯‚ê‚ÎA—n‚©‚·ˆ—‚ğŠJn‚·‚é
        if (!isMelting)
        {
            Debug.Log("ƒŒƒ“ƒW‚ÌInteract()‚ªŒÄ‚Î‚ê‚Ü‚µ‚½I");
            StartMeltingProcess();
        }
        else
        {
            Debug.Log("¡A—n‚©‚µ’†‚Å‚·I");
        }
    }

    private void StartMeltingProcess()
    {

    }

    private void FinishMelting()
    {

    }

}
