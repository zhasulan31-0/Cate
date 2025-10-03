using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class FoodItemData
{
    public string foodName;
    public Sprite foodIcon;
    public GameObject prefab;  // если нужно спавнить 3D или UI версию
}

public class FoodItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Данные о еде")]
    public FoodItemData data;

    [HideInInspector] public bool isFromCoffeeMachine = false;
    [HideInInspector] public CoffeeMachineUI coffeeMachine;

    private Vector3 startPosition;
    private Transform startParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public string GetName() => data != null ? data.foodName : "???";
    public Sprite GetIcon() => data != null ? data.foodIcon : null;
    public GameObject GetPrefab() => data != null ? data.prefab : null;

    // ================= DRAG =================
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        startParent = transform.parent;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            transform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        TrayUI[] trays = FindObjectsOfType<TrayUI>();
        bool addedToTray = false;

        foreach (var tray in trays)
        {
            RectTransform trayRect = tray.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(trayRect, eventData.position, canvas.worldCamera))
            {
                if (tray.AddFood(this))
                {
                    Debug.Log($"Еда {GetName()} добавлена на поднос!");

                    if (isFromCoffeeMachine && coffeeMachine != null)
                    {
                        coffeeMachine.TakeCoffee();
                        isFromCoffeeMachine = false;
                        coffeeMachine = null;
                    }

                    addedToTray = true;
                    break;
                }
            }
        }

        if (!addedToTray)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
        }
    }
}
