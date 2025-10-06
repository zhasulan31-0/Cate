using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class FoodPriceDisplay : MonoBehaviour
{
    [System.Serializable]
    public class FoodPriceUI
    {
        public string foodName;            // �������� ����� (��� � MenuDatabase)
        public TextMeshProUGUI priceText;  // �����, ��� ������������ ����
    }

    [Header("�������� UI ��� ��� ����")]
    public List<FoodPriceUI> foodTexts = new List<FoodPriceUI>();

    [Header("��������� ����������")]
    [Tooltip("��� ����� ��������� ���� (� ��������)")]
    public float refreshInterval = 0.25f;

    private float timer;

    private void Start()
    {
        // ��������� ���������� ��� �������
        UpdateAllPrices();
    }

    private void Update()
    {
        // ������ ���������� ���
        timer += Time.deltaTime;
        if (timer >= refreshInterval)
        {
            timer = 0f;
            UpdateAllPrices();
        }
    }

    private void UpdateAllPrices()
    {
        var menu = MenuDatabase.Instance;
        if (menu == null || menu.foodItems == null || menu.foodItems.Count == 0)
            return;

        foreach (var ui in foodTexts)
        {
            if (ui.priceText == null || string.IsNullOrEmpty(ui.foodName))
                continue;

            var food = menu.foodItems.Find(f => f.foodName == ui.foodName);
            if (food != null)
                ui.priceText.text = $"{food.price}$";
        }
    }
}
