using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // �C���X�y�N�^����PlayerInputManager���Z�b�g����
    public PlayerInputManager playerInputManager;

    // ���̃X�N���v�g���L���ɂȂ������ɌĂ΂��
    private void OnEnable()
    {
        // PlayerInputManager�́u�v���C���[���Q���������v�̃C�x���g�Ɋ֐���o�^����
        playerInputManager.onPlayerJoined += OnPlayerJoined;
    }

    // ���̃X�N���v�g�������ɂȂ������ɌĂ΂��
    private void OnDisable()
    {
        // �o�^�����֐�����������i����@�j
        playerInputManager.onPlayerJoined -= OnPlayerJoined;
    }

    void Update()
    {
        // �u1�v�L�[�������ꂽ��
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // �v���C���[1 (Keyboard_P1����) �Ƃ��ĎQ��������
            playerInputManager.JoinPlayer(playerIndex: 0, controlScheme: "Keyboard_P1");
        }

        // �u2�v�L�[�������ꂽ��
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // �v���C���[2 (Keyboard_P2����) �Ƃ��ĎQ��������
            playerInputManager.JoinPlayer(playerIndex: 1, controlScheme: "Keyboard_P2");
        }
    }

    // ������ �V�����ǉ��������� ������
    // �v���C���[���Q�������u�ԂɁAPlayerInputManager�ɂ���Ď����I�ɌĂяo�����֐�
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("�v���C���[���Q�����܂���: " + playerInput.playerIndex);

        // �Q�������v���C���[���g���Ă���Control Scheme���`�F�b�N
        if (playerInput.currentControlScheme == "Keyboard_P1")
        {
            // ����P1�̑�����@�Ȃ�A"Player"�}�b�v�iWASD�j��L���ɂ���
            Debug.Log("����}�b�v 'Player' ��ݒ肵�܂�");
            playerInput.SwitchCurrentActionMap("Player");
        }
        else if (playerInput.currentControlScheme == "Keyboard_P2")
        {
            // ����P2�̑�����@�Ȃ�A"Player2"�}�b�v�i���L�[�j��L���ɂ���
            Debug.Log("����}�b�v 'Player2' ��ݒ肵�܂�");
            playerInput.SwitchCurrentActionMap("Player2");
        }
    }
}