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

    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        var playerInput = GetComponent<PlayerInput>();

        Debug.Log("actions: " + playerInput.actions); // ← nullじゃないか確認

        moveAction = playerInput.actions["Move"];
        Debug.Log("moveAction: " + moveAction); // ← nullならアクション名ミスの可能性

        playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnDestroy()
    {
        // 登録解除（メモリリーク防止）
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
