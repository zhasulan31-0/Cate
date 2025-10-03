using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TrayUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Точки слотов на подносе")]
    public Transform[] slotPoints;

    private List<FoodItem> storedFoods = new List<FoodItem>();
    private List<string> foodNames = new List<string>(); // храним названия еды

    public int maxSlots = 3;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform startParent;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // Добавляем еду на поднос
    public bool AddFood(FoodItem food)
    {
        if (storedFoods.Count >= maxSlots)
        {
            Debug.Log("Поднос полон!");
            return false;
        }

        storedFoods.Add(food);
        foodNames.Add(food.foodName);

        int index = storedFoods.Count - 1;

        food.transform.SetParent(slotPoints[index], false);
        food.transform.localPosition = Vector3.zero;
        food.transform.localScale = Vector3.one;

        Debug.Log($"Еда {food.foodName} поставлена на слот {index}");
        return true;
    }

    public List<string> GetFoodNames()
    {
        return foodNames;
    }

    public void RemoveFood(int index)
    {
        if (index < 0 || index >= storedFoods.Count) return;

        FoodItem food = storedFoods[index];
        if (food != null)
            Destroy(food.gameObject);

        storedFoods.RemoveAt(index);
        foodNames.RemoveAt(index);
    }

    public bool IsEmpty()
    {
        return storedFoods.Count == 0;
    }

    // ============== DRAG & DROP ==============
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
                Destroy(gameObject); // только если заказ правильный
            else
                Debug.Log("Поднос остаётся, клиент ждёт верный заказ.");
        }
        else
        {
            Debug.Log("Поднос брошен → исчезает");
            Destroy(gameObject);
        }
    }
}
