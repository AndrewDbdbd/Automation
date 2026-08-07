using UnityEngine;
using UnityEngine.InputSystem;

public class WorldObjectDrag : MonoBehaviour
{
    private bool isDragging = false;
    private Collider2D blockCollider;
    private Rigidbody2D rb;
    private Camera mainCamera;

    private void Awake()
    {
        blockCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    public void StartDragging(int id)
    {
        SetDraggingState(true);
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f;
            transform.position = mouseWorldPos;

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                SetDraggingState(false);
            }
        }
        else
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                TrySelectObject();
            }
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                TryDeleteObject();
            }
        }
    }
    private void TryDeleteObject()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            Deleting();
        }
    }
    private void Deleting() 
    {
        Destroy(this.gameObject);
    }
    private void TrySelectObject()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            SetDraggingState(true);
        }
    }

    private void SetDraggingState(bool dragging)
    {
        isDragging = dragging;

        if (blockCollider != null)
        {
            blockCollider.enabled = !dragging;
        }

        if (rb != null)
        {
            rb.isKinematic = dragging;
            if (dragging) rb.linearVelocity = Vector2.zero;
        }
    }
}
