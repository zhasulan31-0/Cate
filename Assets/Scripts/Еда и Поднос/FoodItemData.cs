using UnityEngine;

[CreateAssetMenu(fileName = "NewFoodItem", menuName = "Cafe/FoodItemData")]
public class FoodItemData : ScriptableObject
{
    [Header("Основные данные")]
    public string foodName;
    public Sprite foodIcon;

    [Header("Префабы")]
    public GameObject prefabUI;   // версия для UI/подноса

    [Header("Дополнительно")]
    public AudioClip eatSound;
    public int price = 10;

    // --- сохранение оригинальной цены ---
    [HideInInspector] public int defaultPrice;

    private void OnEnable()
    {
        if (defaultPrice == 0) defaultPrice = price;
    }

    public void ResetToDefault()
    {
        price = defaultPrice;
    }
}
