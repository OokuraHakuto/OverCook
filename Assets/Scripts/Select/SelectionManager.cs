using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    // ���̃N���X�̗B��̃C���X�^���X�i�V���O���g���j
    public static SelectionManager instance;

    // --- �I�����ꂽ����ێ�����ϐ� ---
    public GameObject player1Prefab; // P1���I�񂾃L�����̃v���t�@�u
    public GameObject player2Prefab; // P2���I�񂾃L�����̃v���t�@�u

    void Awake()
    {
        // �V�[�����܂����Ŏ������g��ێ�����
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
