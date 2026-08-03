using System;
using System.Collections;
using Unity.VisualScripting;
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
    private bool isBuffered = false;
    private bool coyoteTimed = false;
    [Range(0f, 1f)]
    [SerializeField] private float jumpCutMultiplier = 1f;
    [SerializeField] private float bufferDuration = 0.2f;
    [SerializeField] private float coyoteDuration = 0.5f;
    [SerializeField] private GameObject GroundCheckPoint;
    private float coyoteTimeCounter = 0f;
    void Start()
    {
        jumpAction = InputSystem.actions.FindAction("Jump");
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction.performed += JumpAction_performed;
        jumpAction.canceled += JumpAction_canceled;
        rb = GetComponent<Rigidbody2D>();
        bc = GroundCheckPoint.GetComponent<BoxCollider2D>();
        if (moveAction == null)
            Debug.LogError("Move action not found!");
        if (rb == null)
            Debug.LogError("Rigidbody2D not found!");
        coyoteTimed = true;
        moveAction.Enable();
    }

    private void JumpAction_performed(InputAction.CallbackContext context)
    {
        if (IsGrounded()||coyoteTimed)
        {
            //Debug.Log(IsGrounded() +" "+ coyoteTimed);
            coyoteTimeCounter = 0f;
            Jump();
        }
        else if (!isBuffered)
        {
            isBuffered = true;
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
    void FixedUpdate()
    {
        if (moveAction == null) return;

        float moveInput = moveAction.ReadValue<float>();
        float targetSpeed = moveInput * moveSpeed;
        currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(currentHorizontalSpeed, rb.linearVelocity.y);
    }
    private void Update()
    {
        if (IsGrounded()) coyoteTimeCounter = coyoteDuration;
        else coyoteTimeCounter -= Time.deltaTime;


        if (coyoteTimeCounter > 0f)
        {
            coyoteTimed = true;

        }
        else { coyoteTimed = false; }
    }
    IEnumerator BufferTimer()
    {
        float timePassed = 0;
        while (timePassed < bufferDuration)
        {
            yield return waitForFixedUpdate;

            timePassed += Time.fixedDeltaTime;

            if (IsGrounded())
            {
                Jump();
                isBuffered = false;
                yield break;
            }
        }

        isBuffered = false;
    }
    bool IsGrounded() 
    {
        return bc.IsTouchingLayers(whatIsGround);
    }


}
