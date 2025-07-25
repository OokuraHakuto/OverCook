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

    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        var playerInput = GetComponent<PlayerInput>();

        Debug.Log("actions: " + playerInput.actions); // �� null����Ȃ����m�F

        moveAction = playerInput.actions["Move"];
        Debug.Log("moveAction: " + moveAction); // �� null�Ȃ�A�N�V�������~�X�̉\��

        playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnDestroy()
    {
        // �o�^�����i���������[�N�h�~�j
        var playerInput = GetComponent<PlayerInput>();

        playerInput.onActionTriggered -= OnActionTriggered;
    }

    private void OnActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action == moveAction)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);

        rb.velocity = movement * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }
}
