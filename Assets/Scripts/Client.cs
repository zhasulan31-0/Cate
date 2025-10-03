using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Client : MonoBehaviour
{
    [Header("UI заказа")]
    public GameObject orderBubble;
    public Transform orderIconsParent;    // контейнер внутри Bubble
    public GameObject orderIconPrefab;    // префаб иконки (Image + Text)

    private List<FoodItemData> currentOrder = new List<FoodItemData>();

    private void Start()
    {
        MakeOrder();
    }

    void MakeOrder()
    {
        currentOrder.Clear();

        int count = Random.Range(1, 4); // от 1 до 3
        for (int i = 0; i < count; i++)
        {
            var item = MenuDatabase.Instance.GetRandomItem();
            if (item != null)
                currentOrder.Add(item);
        }

        // группируем одинаковые блюда
        var grouped = currentOrder
            .GroupBy(f => f.foodName)
            .Select(g => new { food = g.First(), count = g.Count() })
            .ToList();

        // очищаем старые иконки
        foreach (Transform child in orderIconsParent)
            Destroy(child.gameObject);

        // создаем иконки заказа
        foreach (var g in grouped)
        {
            GameObject iconObj = Instantiate(orderIconPrefab, orderIconsParent);

            Image foodIcon = iconObj.GetComponentInChildren<Image>();
            if (foodIcon != null)
                foodIcon.sprite = g.food.foodIcon;

            Text countText = iconObj.GetComponentInChildren<Text>();
            if (countText != null)
                countText.text = g.count > 1 ? $"x{g.count}" : "";
        }

        orderBubble.SetActive(true);

        Debug.Log($"Клиент заказал: {string.Join(", ", currentOrder.Select(f => f.foodName))}");
    }

    public bool CheckOrder(TrayUI tray)
    {
        List<string> trayFoods = tray.GetFoodNames();
        List<string> orderFoods = currentOrder.Select(f => f.foodName).ToList();

        // проверяем количество каждого блюда
        bool equal = trayFoods.Count == orderFoods.Count &&
                     trayFoods.GroupBy(x => x).All(g => orderFoods.Count(f => f == g.Key) == g.Count());

        if (equal)
        {
            Debug.Log("Клиент доволен! Заказ выполнен.");
            Leave();
            return true;
        }

        Debug.Log("Неправильный заказ! Клиент ждет ещё раз.");
        return false;
    }

    void Leave()
    {
        orderBubble.SetActive(false);
        Debug.Log("Клиент ушел довольный!");
        Destroy(gameObject, 1f);
    }
}
