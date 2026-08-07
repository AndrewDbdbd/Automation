using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject blockPrefab;

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        GameObject newBlock = Instantiate(blockPrefab, mouseWorldPos, Quaternion.identity);

        WorldObjectDrag dragScript = newBlock.GetComponent<WorldObjectDrag>();
        if (dragScript == null)
        {
            dragScript = newBlock.AddComponent<WorldObjectDrag>();
        }

        dragScript.StartDragging(eventData.pointerId);
    }
}