using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    [Header("UI заказа")]
    public GameObject orderBubble;
    public Image orderImage;

    private MenuItem currentOrder;

    private void Start()
    {
        MakeOrder();
    }

    void MakeOrder()
    {
        currentOrder = MenuDatabase.Instance.GetRandomItem();

        if (currentOrder != null)
        {
            orderImage.sprite = currentOrder.foodIcon;
            orderBubble.SetActive(true);

            Debug.Log($"Клиент заказал: {currentOrder.foodName}");
        }
    }

    public bool CheckOrder(TrayUI tray)
    {
        List<string> foods = tray.GetFoodNames();

        foreach (var foodName in foods)
        {
            if (foodName == currentOrder.foodName)
            {
                Debug.Log("Клиент доволен! Заказ выполнен.");
                Leave();
                return true;
            }
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
