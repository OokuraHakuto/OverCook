using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSettings : MonoBehaviour
{
    [Header("机や台に置いた時のスケール")]
    public Vector3 onTableScale = Vector3.one; // デフォルトは (1, 1, 1)

    // 将来的に「持った時のスケール」や「回転の補正」などもここに追加できます
    [Tooltip("プレイヤーが持ったときのスケール")]
    public Vector3 onPlayerScale = Vector3.one;
}