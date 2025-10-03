using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class TrayUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private List<FoodItem> storedFoods = new List<FoodItem>();
    private List<FoodItemData> storedData = new List<FoodItemData>(); // <--- теперь есть список данных

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform startParent;

    private RectTransform rectTransform;
    private HorizontalLayoutGroup layoutGroup;

    [SerializeField] private float padding = 10f;
    [SerializeField] private float maxSpacing = 10f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
            layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();

        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = false;
        layoutGroup.childForceExpandWidth = false;

        canvas = FindObjectOfType<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public bool AddFood(FoodItem food)
    {
        if (storedFoods.Contains(food))
            return false;

        food.transform.SetParent(transform, false);
        food.transform.localScale = Vector3.one;

        storedFoods.Add(food);
        if (food.data != null)
            storedData.Add(food.data);

        AdjustSpacing();

        Debug.Log($"Еда {food.GetName()} поставлена на поднос (всего {storedFoods.Count})");
        return true;
    }

    private void AdjustSpacing()
    {
        int count = storedFoods.Count;
        if (count == 0) return;

        float trayWidth = rectTransform.rect.width - padding * 2;
        float totalFoodWidth = 0f;

        foreach (var food in storedFoods)
        {
            RectTransform rt = food.GetComponent<RectTransform>();
            totalFoodWidth += rt.rect.width;
        }

        float spacing = 0f;
        if (count > 1)
            spacing = (trayWidth - totalFoodWidth) / (count - 1);

        if (spacing > maxSpacing)
            spacing = maxSpacing;

        layoutGroup.spacing = spacing;
        layoutGroup.padding.left = (int)padding;
        layoutGroup.padding.right = (int)padding;
    }

    public void RemoveFood(FoodItem food)
    {
        if (storedFoods.Contains(food))
        {
            int index = storedFoods.IndexOf(food);
            storedFoods.RemoveAt(index);
            if (index < storedData.Count) storedData.RemoveAt(index);

            Destroy(food.gameObject);
            AdjustSpacing();
        }
    }

    public List<string> GetFoodNames()
    {
        List<string> names = new List<string>();
        foreach (var d in storedData)
        {
            if (d != null) names.Add(d.foodName);
        }
        return names;
    }

    public List<FoodItemData> GetFoodData() => new List<FoodItemData>(storedData);

    public bool IsEmpty() => storedFoods.Count == 0;

    // ================= DRAG & DROP =================
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

        Client client = eventData.pointerEnter ? eventData.pointerEnter.GetComponentInParent<Client>() : null;

        if (client != null)
        {
            Debug.Log("Поднос передан клиенту!");
            bool success = client.CheckOrder(this);

            if (success)
            {
                Destroy(gameObject);
                return;
            }

            Debug.Log("Поднос остаётся, клиент ждёт верный заказ.");
        }

        transform.position = startPosition;
        transform.SetParent(startParent);
    }
}
