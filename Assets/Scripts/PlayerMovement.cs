using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private InputAction moveAction;
    private InputAction jumpAction;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float accelerationCurve = 17f;
    [SerializeField] private float stoppingForce = 15f; // Сила торможения

    void Start()
    {
        jumpAction = InputSystem.actions.FindAction("Jump");
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction.performed += JumpAction_performed;
        rb = GetComponent<Rigidbody2D>();
        if (moveAction == null)
            Debug.LogError("Move action not found!");
        if (rb == null)
            Debug.LogError("Rigidbody2D not found!");
    }

    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        rb.AddForce(Vector2.up * jumpForce,ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        if (moveAction == null) return;

        moveInput = moveAction.ReadValue<Vector2>();

        if (moveInput.magnitude > 0.01f)
        {

            // Нормализуем ввод
            Vector2 inputDirection = moveInput.normalized;

            // Преобразуем в мировое направление с учётом поворота объекта
            Vector2 moveDirection = (transform.right * inputDirection.x);

            // Получаем текущую скорость в направлении движения
            float currentSpeedInDirection = Vector2.Dot(rb.linearVelocity, moveDirection);

            // Вычисляем целевую скорость по кривой
            float targetSpeed;
            if (currentSpeedInDirection <= 0.01f)
            {
                targetSpeed = Mathf.Sqrt(accelerationCurve * Time.fixedDeltaTime);
            }
            else
            {
                targetSpeed = Mathf.Sqrt(accelerationCurve * currentSpeedInDirection);
            }

            targetSpeed = Mathf.Min(targetSpeed, maxSpeed);

            // Вычисляем желаемую скорость
            Vector2 desiredVelocity = moveDirection * targetSpeed;

            // Применяем силу для достижения желаемой скорости
            Vector2 force = (desiredVelocity - rb.linearVelocity) * accelerationCurve;
            rb.AddForce(force, ForceMode2D.Force);

            // Ограничиваем максимальную скорость (на случай слишком большой силы)
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            // Торможение с помощью силы
            rb.AddForce(-rb.linearVelocity * stoppingForce, ForceMode2D.Force);
        }
    }
}
