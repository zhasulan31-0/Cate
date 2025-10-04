using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    public FoodItemData GetUniqueRandomItem(List<FoodItemData> source = null)
    {
        List<FoodItemData> list = source ?? foodPrefabs.Select(f => f.data).ToList();
        // делаем уникальные по имени
        var uniqueList = list.GroupBy(f => f.foodName).Select(g => g.First()).ToList();
        if (uniqueList.Count == 0) return null;
        int index = Random.Range(0, uniqueList.Count);
        return uniqueList[index];
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
