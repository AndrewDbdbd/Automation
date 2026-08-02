using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float moveDistance = 5f;

    [Header("Граница")]
    [SerializeField] private PolygonCollider2D boundsCollider;
    private Camera cam;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    void LateUpdate()
    {
        if (target == null) return;
        float currentSpeed = smoothSpeed;
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > moveDistance)
        {
            currentSpeed *= 1f + (distance - moveDistance);
        }
        //float t = Mathf.Min(currentSpeed * Time.deltaTime, 1f);
        Vector2 targetVec2 = Vector2.Lerp(transform.position, target.position, currentSpeed * Time.deltaTime);
        if (boundsCollider != null)
        {
            // Рассчитываем половину высоты камеры в мировых единицах
            float camHalfHeight = cam.orthographicSize;
            // Рассчитываем половину ширины на основе соотношения сторон экрана
            float camHalfWidth = camHalfHeight * cam.aspect;

            // Проверяем четыре крайние точки камеры (Углы / Края)
            // и корректируем позицию центра, если какой-то из краев вылетает наружу

            // Проверка левого и правого краев
            Vector2 leftEdge = new Vector2(targetVec2.x - camHalfWidth, targetVec2.y);
            Vector2 rightEdge = new Vector2(targetVec2.x + camHalfWidth, targetVec2.y);

            Vector2 clampedLeft = boundsCollider.ClosestPoint(leftEdge);
            Vector2 clampedRight = boundsCollider.ClosestPoint(rightEdge);

            // Если левый край вышел за предел, сдвигаем центр вправо
            if (leftEdge != clampedLeft) targetVec2.x += (clampedLeft.x - leftEdge.x);
            // Если правый край вышел за предел, сдвигаем центр влево
            if (rightEdge != clampedRight) targetVec2.x += (clampedRight.x - rightEdge.x);

            // Проверка нижнего и верхнего краев
            Vector2 bottomEdge = new Vector2(targetVec2.x, targetVec2.y - camHalfHeight);
            Vector2 topEdge = new Vector2(targetVec2.x, targetVec2.y + camHalfHeight);

            Vector2 clampedBottom = boundsCollider.ClosestPoint(bottomEdge);
            Vector2 clampedTop = boundsCollider.ClosestPoint(topEdge);

            // Если нижний край вышел за предел, сдвигаем центр вверх
            if (bottomEdge != clampedBottom) targetVec2.y += (clampedBottom.y - bottomEdge.y);
            // Если верхний край вышел за предел, сдвигаем центр вниз
            if (topEdge != clampedTop) targetVec2.y += (clampedTop.y - topEdge.y);
        }
        transform.position = new Vector3(targetVec2.x, targetVec2.y, transform.position.z);

    }
}
