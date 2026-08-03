using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float delay = 5f;

    [Header("Граница")]
    [SerializeField] private PolygonCollider2D boundsCollider;
    private Camera cam;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    void LateUpdate()
    {
        Vector3 delayVec = new Vector3(0, -delay, 0);
        if (target == null) return;
        float currentSpeed = smoothSpeed;
        float distance = Vector2.Distance(transform.position, target.position - delayVec);
        if (distance > moveDistance)
        {
            currentSpeed *= 1f + (distance - moveDistance);
        }
        Vector2 targetVec2 = Vector2.Lerp(transform.position, target.position - delayVec, currentSpeed * Time.deltaTime);
        if (boundsCollider != null)
        {
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = camHalfHeight * cam.aspect;

            Vector2 leftEdge = new Vector2(targetVec2.x - camHalfWidth, targetVec2.y);
            Vector2 rightEdge = new Vector2(targetVec2.x + camHalfWidth, targetVec2.y);

            Vector2 clampedLeft = boundsCollider.ClosestPoint(leftEdge);
            Vector2 clampedRight = boundsCollider.ClosestPoint(rightEdge);

            if (leftEdge != clampedLeft) targetVec2.x += (clampedLeft.x - leftEdge.x);
            if (rightEdge != clampedRight) targetVec2.x += (clampedRight.x - rightEdge.x);

            Vector2 bottomEdge = new Vector2(targetVec2.x, targetVec2.y - camHalfHeight);
            Vector2 topEdge = new Vector2(targetVec2.x, targetVec2.y + camHalfHeight);

            Vector2 clampedBottom = boundsCollider.ClosestPoint(bottomEdge);
            Vector2 clampedTop = boundsCollider.ClosestPoint(topEdge);

            if (bottomEdge != clampedBottom) targetVec2.y += (clampedBottom.y - bottomEdge.y);
            if (topEdge != clampedTop) targetVec2.y += (clampedTop.y - topEdge.y);
        }
        transform.position = new Vector3(targetVec2.x, targetVec2.y, transform.position.z);

    }
}

//sixseven