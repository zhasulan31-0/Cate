using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class FoodPriceDisplay : MonoBehaviour
{
    [System.Serializable]
    public class FoodPriceUI
    {
        public string foodName;            // Название блюда (как в MenuDatabase)
        public TextMeshProUGUI priceText;  // Текст, где показывается цена
    }

    [Header("Элементы UI для цен блюд")]
    public List<FoodPriceUI> foodTexts = new List<FoodPriceUI>();

    [Header("Настройки обновления")]
    [Tooltip("Как часто обновлять цены (в секундах)")]
    public float refreshInterval = 0.25f;

    private float timer;

    private void Start()
    {
        // Первичное обновление при запуске
        UpdateAllPrices();
    }

    private void Update()
    {
        // Таймер обновления цен
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
