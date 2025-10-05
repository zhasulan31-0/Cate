using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuDatabase : MonoBehaviour
{
    public static MenuDatabase Instance;

    [Header("Список доступных блюд (ScriptableObject)")]
    public List<FoodItemData> foodItems = new List<FoodItemData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public FoodItemData GetUniqueRandomItem(List<FoodItemData> source = null)
    {
        List<FoodItemData> list = source ?? foodItems;
        var uniqueList = list.GroupBy(f => f.foodName).Select(g => g.First()).ToList();
        if (uniqueList.Count == 0) return null;
        int index = Random.Range(0, uniqueList.Count);
        return uniqueList[index];
    }

    public FoodItemData GetItemByName(string name)
    {
        return foodItems.FirstOrDefault(f => f.foodName == name);
    }
}
