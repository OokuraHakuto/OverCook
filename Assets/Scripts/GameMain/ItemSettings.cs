using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSettings : MonoBehaviour
{
    [Header("サイズ設定")]
    [Tooltip("机に置いたときのスケール")]
    public Vector3 onTableScale = Vector3.one;

    [Tooltip("プレイヤーが持ったときのスケール")]
    public Vector3 onPlayerScale = Vector3.one;

    [Header("位置補正")]
    [Tooltip("手で持った時の位置オフセット（ピボットズレ修正用）")]
    public Vector3 holdPositionOffset = Vector3.zero;

    [Header("回転設定")]
    [Tooltip("プレイヤーが持った時の回転角度 (例: 0, 0, 0)")]
    public Vector3 onPlayerRotation = Vector3.zero;
}