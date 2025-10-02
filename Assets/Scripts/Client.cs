using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    [Header("UI ������")]
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

            Debug.Log($"������ �������: {currentOrder.foodName}");
        }
    }

    public bool CheckOrder(TrayUI tray)
    {
        List<string> foods = tray.GetFoodNames();

        foreach (var foodName in foods)
        {
            if (foodName == currentOrder.foodName)
            {
                Debug.Log("������ �������! ����� ��������.");
                Leave();
                return true;
            }
        }

        Debug.Log("������������ �����! ������ ���� ��� ���.");
        return false;
    }

    void Leave()
    {
        orderBubble.SetActive(false);
        Debug.Log("������ ���� ���������!");
        Destroy(gameObject, 1f);
    }
}
