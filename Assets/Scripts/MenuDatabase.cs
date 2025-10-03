using UnityEngine;
using System.Collections.Generic;

public class MenuDatabase : MonoBehaviour
{
    public static MenuDatabase Instance;

    [Header("Список доступных блюд (префабы FoodItem)")]
    public List<FoodItem> foodPrefabs = new List<FoodItem>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public FoodItemData GetRandomItem()
    {
        if (foodPrefabs.Count == 0) return null;
        int index = Random.Range(0, foodPrefabs.Count);
        return foodPrefabs[index].data;
    }

    public FoodItemData GetItemByName(string name)
    {
        foreach (var f in foodPrefabs)
        {
            if (f.data.foodName == name) return f.data;
        }
        return null;
    }
}
