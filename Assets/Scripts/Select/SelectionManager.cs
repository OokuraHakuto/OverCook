using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    // このクラスの唯一のインスタンス（シングルトン）
    public static SelectionManager instance;

    // --- 選択された情報を保持する変数 ---
    public GameObject player1Prefab; // P1が選んだキャラのプレファブ
    public GameObject player2Prefab; // P2が選んだキャラのプレファブ

    void Awake()
    {
        // シーンをまたいで自分自身を保持する
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
