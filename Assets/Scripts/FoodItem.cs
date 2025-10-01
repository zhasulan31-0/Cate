using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private Transform startParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public Sprite foodIcon; // ������ ��� UI �������

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        startParent = transform.parent;

        transform.SetParent(canvas.transform);

        // ������ ������
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint))
        {
            transform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ���������� ������� ����������� ������ raycast
        canvasGroup.blocksRaycasts = true;

        // ��������� � ����� �� �� ������
        TrayUI tray = eventData.pointerEnter ? eventData.pointerEnter.GetComponentInParent<TrayUI>() : null;

        if (tray != null && tray.AddFood(foodIcon))
        {
            Debug.Log("��� ��������� �� ������: " + foodIcon.name);
            Destroy(gameObject); // ������� ���������� ��� �� �����
        }
        else
        {
            // ���������� �������
            transform.position = startPosition;
            transform.SetParent(startParent);
        }
    }
}
