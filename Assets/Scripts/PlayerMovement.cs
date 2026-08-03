using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private InputAction moveAction;
    private InputAction jumpAction;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 17f;
    private float currentHorizontalSpeed = 0f;

    public LayerMask whatIsGround;
    private bool isGrounded;
    private bool isBuffered = false;
    private float timePassed = 0f;

    [Range(0f, 1f)]
    [SerializeField] private float jumpCutMultiplier = 1f;
    [SerializeField] private float bufferDuration = 0.2f;
    void Start()
    {
        jumpAction = InputSystem.actions.FindAction("Jump");
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction.performed += JumpAction_performed;
        jumpAction.canceled += JumpAction_canceled;
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        if (moveAction == null)
            Debug.LogError("Move action not found!");
        if (rb == null)
            Debug.LogError("Rigidbody2D not found!");
        moveAction.Enable();
    }

    private void JumpAction_performed(InputAction.CallbackContext context)
    {
        isGrounded = bc.IsTouchingLayers(whatIsGround);
        if (isGrounded)
        {
            Jump();
        }
        else if (!isBuffered)
        {
            isBuffered= true;
            StartCoroutine(BufferTimer());
        }
    }
    private void JumpAction_canceled(InputAction.CallbackContext context)
    {
        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    void Update()
    {
        //sixseven
    }
    void FixedUpdate()
    {
        if (moveAction == null) return;

        float moveInput = moveAction.ReadValue<float>();
        float targetSpeed = moveInput * moveSpeed;
        currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(currentHorizontalSpeed, rb.linearVelocity.y);
    }
    IEnumerator BufferTimer()
    {
        timePassed = 0;
        while (timePassed < bufferDuration)
        {
            yield return waitForFixedUpdate;

            timePassed += Time.fixedDeltaTime;

            isGrounded = bc.IsTouchingLayers(whatIsGround);
            if (isGrounded)
            {
                Jump();
                isBuffered = false;
                yield break;
            }
        }

        isBuffered = false;
    }


}
