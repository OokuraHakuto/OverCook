using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //�v���C���[�̈ړ����x�i�C���X�y�N�^�[�Œ����j
    public float moveSpeed = 5.0f;

    //PlayerInput
    private Vector2 moveInput;

    //Rigidbody�ւ̎Q��
    private Rigidbody rb;

    //InputActions ��ێ�����ϐ�
    private PlayerInputActions inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        inputActions = new PlayerInputActions();

        // ���̓C�x���g�o�^�iMove�����������Ƃ��j
        inputActions.Player.Move.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>();
        };
        inputActions.Player.Move.canceled += ctx =>
        {
            moveInput = Vector2.zero;
        };
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        rb.velocity = movement * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }
}
