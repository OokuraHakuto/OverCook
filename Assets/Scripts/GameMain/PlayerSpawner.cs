using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("�v���C���[1�̏o���ʒu")]
    public Transform player1SpawnPoint; // P1���X�|�[������ꏊ

    [Header("�v���C���[2�̏o���ʒu")]
    public Transform player2SpawnPoint; // P2���X�|�[������ꏊ

    void Start()
    {
        if (SelectionManager.instance != null)
        {
            // ---------------------------------
            // �v���C���[1���X�|�[��������
            // ---------------------------------
            GameObject p1Prefab = SelectionManager.instance.player1Prefab;
            if (p1Prefab != null)
            {
                // 1. ��������P1�I�u�W�F�N�g��ϐ��Ɋi�[
                GameObject player1Object = Instantiate(p1Prefab, player1SpawnPoint.position, player1SpawnPoint.rotation);

                // 2. P1�I�u�W�F�N�g����PlayerController�X�N���v�g���擾
                PlayerController p1Controller = player1Object.GetComponent<PlayerController>();
                if (p1Controller != null)
                {
                    // 3. �f�o�b�O�p��playerID���u1�v�ɐݒ�
                    p1Controller.playerID = 1;
                }
                Debug.Log("�v���C���[1: " + p1Prefab.name + " ���X�|�[�����܂��� (ID: 1)");
            }
            else
            {
                Debug.LogWarning("�v���C���[1�̃L�����N�^�[���I������Ă��܂���I");
            }

            // ---------------------------------
            // �v���C���[2���X�|�[��������
            // ---------------------------------
            GameObject p2Prefab = SelectionManager.instance.player2Prefab;
            if (p2Prefab != null)
            {
                // 1. ��������P2�I�u�W�F�N�g��ϐ��Ɋi�[
                GameObject player2Object = Instantiate(p2Prefab, player2SpawnPoint.position, player2SpawnPoint.rotation);

                // 2. P2�I�u�W�F�N�g����PlayerController�X�N���v�g���擾
                PlayerController p2Controller = player2Object.GetComponent<PlayerController>();
                if (p2Controller != null)
                {
                    // 3. �f�o�b�O�p��playerID���u2�v�ɐݒ�
                    p2Controller.playerID = 2;
                }
                Debug.Log("�v���C���[2: " + p2Prefab.name + " ���X�|�[�����܂��� (ID: 2)");
            }
            else
            {
                Debug.LogWarning("�v���C���[2�̃L�����N�^�[���I������Ă��܂���I");
            }
        }
        else
        {
            Debug.LogError("�L�����N�^�[�I�����iSelectionManager�j��������܂���I");
        }
    }
}