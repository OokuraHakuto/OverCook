using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerSpawner : MonoBehaviour
{
    //Unityエディタ上で設定できるプレイヤーの出現位置のリスト。
    public Transform[] spawnPoints;

    //プレイヤー
    private int currentSpawnIndex = 0;

    //このスクリプトが有効になるとき（シーン開始時など）に,
    //PlayerInputManagerに「プレイヤーが参加したら OnPlayerJoined を呼んでね」と登録している。
    private void OnEnable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        }
        else
        {
            Debug.LogWarning("PlayerInputManager が見つかりませんでした！");
        }
    }

    //実際にそのプレイヤーを指定のスポーン位置に移動させる。
    private void OnDisable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        }
        else
        {
            Debug.LogWarning("PlayerInputManager が見つかりませんでした！");
        }
    }

    //新しいプレイヤーが参加したときに自動的に呼ばれる関数。
    //引数の playerInput は、生成されたプレイヤーオブジェクト（Clone）にアクセスするためのもの。
    private void OnPlayerJoined(PlayerInput playerInput)
    { 
        if (currentSpawnIndex < spawnPoints.Length)
        {
            Vector3 spawnPos = spawnPoints[currentSpawnIndex].position;

            playerInput.transform.position = spawnPoints[currentSpawnIndex].position;

            Debug.Log($"Player {currentSpawnIndex} spawned at {spawnPos}");

            currentSpawnIndex++;
        }
        else
        {
            Debug.LogWarning("スポーン地点が足りません！");
        }
    }
}
