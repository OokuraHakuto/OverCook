using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //プレイヤーの移動速度（インスペクターで調整）
    public float moveSpeed = 5.0f;

    //PlayerInput
    private Vector2 moveInput;

    //Rigidbodyへの参照
    private Rigidbody rb;

    //InputActions を保持する変数
    private PlayerInputActions inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        inputActions = new PlayerInputActions();

        // 入力イベント登録（Moveが発生したとき）
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
