using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerSpawner : MonoBehaviour
{
    //Unity�G�f�B�^��Őݒ�ł���v���C���[�̏o���ʒu�̃��X�g�B
    public Transform[] spawnPoints;

    //�v���C���[
    private int currentSpawnIndex = 0;

    //���̃X�N���v�g���L���ɂȂ�Ƃ��i�V�[���J�n���Ȃǁj��,
    //PlayerInputManager�Ɂu�v���C���[���Q�������� OnPlayerJoined ���Ă�łˁv�Ɠo�^���Ă���B
    private void OnEnable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        }
        else
        {
            Debug.LogWarning("PlayerInputManager ��������܂���ł����I");
        }
    }

    //���ۂɂ��̃v���C���[���w��̃X�|�[���ʒu�Ɉړ�������B
    private void OnDisable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        }
        else
        {
            Debug.LogWarning("PlayerInputManager ��������܂���ł����I");
        }
    }

    //�V�����v���C���[���Q�������Ƃ��Ɏ����I�ɌĂ΂��֐��B
    //������ playerInput �́A�������ꂽ�v���C���[�I�u�W�F�N�g�iClone�j�ɃA�N�Z�X���邽�߂̂��́B
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
            Debug.LogWarning("�X�|�[���n�_������܂���I");
        }
    }
}
