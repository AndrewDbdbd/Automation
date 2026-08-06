using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
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
    [SerializeField] private BackpackLogic backpackLogic;
    [SerializeField] private float jumpWaterCost = 5f;
    [SerializeField] private float walkWaterCost= 5f;
    private float coyoteTimeCounter = 0f;
    private bool isFacingRight;
    private float count = 1;
    private bool isJumping = false;
    private void Awake() => instance = this;
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
        isFacingRight = true;
    }

    private void JumpAction_performed(InputAction.CallbackContext context)
    {
        if (IsGrounded()|| (coyoteTimed && coyoteTimeCounter > 0f))
        {
            //Debug.Log(IsGrounded() +" "+ coyoteTimed+" "+coyoteTimeCounter ); 
            coyoteTimeCounter = 0f;
            coyoteTimed = false;
            isJumping = true;
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
            isJumping = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        backpackLogic.Decrease(jumpWaterCost);
    }
    void FixedUpdate()
    {
        if (moveAction == null) return;


        float moveInput = moveAction.ReadValue<float>();
        float targetSpeed = moveInput * moveSpeed;
        if ((moveInput > 0) != (transform.localScale.x > 0)&& Math.Abs(moveInput) > 0.1f)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        if (Mathf.Abs(moveInput) > 0.1f && IsGrounded())
        {
            float walkCostThisFrame = Mathf.Abs(moveInput) * walkWaterCost * Time.fixedDeltaTime;
            backpackLogic.Decrease(walkCostThisFrame);
        }
        if (isJumping && rb.linearVelocity.y > 0.1f)
        {
            float jumpHoldCostThisFrame = jumpWaterCost * Time.fixedDeltaTime;
            backpackLogic.Decrease(jumpHoldCostThisFrame);
        }
        currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(currentHorizontalSpeed, rb.linearVelocity.y);

    }
    private void Update()
    {
        isJumping = !IsGrounded();
        if (IsGrounded() && rb.linearVelocity.y <= 0.1f)
        {
            coyoteTimeCounter = coyoteDuration;
            coyoteTimed = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            if (coyoteTimeCounter <= 0f)
            {
                coyoteTimed = false;
            }
        }
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
