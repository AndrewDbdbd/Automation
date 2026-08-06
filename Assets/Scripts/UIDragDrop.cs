using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UIDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 startPosition;
    private Transform startParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Находим главный Canvas в сцене
        canvas = GetComponentInParent<Canvas>();

        // Добавляем CanvasGroup программно, если его нет (нужен для прозрачности и игнорирования лучей)
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    // Срабатывает в момент нажатия и начала движения мыши
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Запоминаем начальную позицию и родителя, чтобы вернуть объект, если его отпустят не туда
        startPosition = rectTransform.anchoredPosition;
        startParent = transform.parent;

        // Делаем объект полупрозрачным во время таскания
        canvasGroup.alpha = 0.6f;

        // Отключаем блокировку лучей, чтобы мышь могла "видеть" объекты под иконкой
        canvasGroup.blocksRaycasts = false;

        // Выносим объект на самый верх иерархии Canvas, чтобы он не прятался за другими панелями
        transform.SetParent(canvas.transform, true);
    }

    // Срабатывает каждый кадр, пока зажата мышь и идет движение
    public void OnDrag(PointerEventData eventData)
    {
        // Двигаем UI элемент вслед за курсором с учетом масштаба Canvas
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // Срабатывает, когда игрок отпускает кнопку мыши
    public void OnEndDrag(PointerEventData eventData)
    {
        // Возвращаем прозрачность и плотность для лучей
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Возвращаем иконку на исходное место в панели
        transform.SetParent(startParent, false);
        rectTransform.anchoredPosition = startPosition;
    }
}