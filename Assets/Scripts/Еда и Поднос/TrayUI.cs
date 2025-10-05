using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrayUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action onTrayDestroyed;

    private List<FoodItem> storedFoods = new List<FoodItem>();
    private List<FoodItemData> storedData = new List<FoodItemData>(); // <--- теперь есть список данных
    public List<FoodItemData> GetFoodData() => new List<FoodItemData>(storedData);

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

        var cg = food.GetComponent<CanvasGroup>();
        if (cg != null) cg.blocksRaycasts = true;
        food.enabled = false;

        // ⚡ Событие: еда добавлена на поднос
        food.OnPlacedOnTray();

        AdjustSpacing();
        Debug.Log($"Еда {food.GetName()} поставлена на поднос. На подносе теперь {storedFoods.Count} ед.");
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

    public void DestroyTray()
    {
        // уничтожаем всё, что на нём лежит
        foreach (var food in storedFoods)
        {
            if (food != null) Destroy(food.gameObject);
        }
        storedFoods.Clear();
        storedData.Clear();

        onTrayDestroyed?.Invoke(); // 🔔 сообщаем спавнеру
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        onTrayDestroyed?.Invoke();
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
    public int GetTotalPrice()
    {
        int total = 0;
        foreach (var item in storedData)
        {
            total += item.price;
        }
        return total;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        Client client = eventData.pointerEnter ? eventData.pointerEnter.GetComponentInParent<Client>() : null;

        if (client != null)
        {
            // Список еды
            List<string> foods = GetFoodNames();
            if (foods.Count == 0)
            {
                Debug.Log("Поднос передан клиенту! На подносе пусто.");
            }
            else
            {
                Debug.Log($"Поднос передан клиенту! На подносе: {string.Join(", ", foods)}");
            }

            client.CheckOrder(this);

            // Поднос всегда уходит с клиентом
            Destroy(gameObject);
            return;
        }

        // если клиента нет → вернуть на место
        transform.position = startPosition;
        transform.SetParent(startParent);
    }


}
