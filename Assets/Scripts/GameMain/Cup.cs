using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : MonoBehaviour
{
    [Header("カップの状態")]
    // これが false なら「空のカップ」、true なら「完成したアイス」
    public bool isFull = false;

    [Header("アイスの種類")]
    // "Vanilla", "Chocolate", "Matcha", "Strawberry" など
    public string flavor = "Vanilla";
}
