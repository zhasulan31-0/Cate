using UnityEngine;
using System.Collections.Generic;


public class MenuDatabase : MonoBehaviour
{
    public static MenuDatabase Instance;

    public List<MenuItem> menuItems = new List<MenuItem>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public MenuItem GetRandomItem()
    {
        if (menuItems.Count == 0) return null;
        int index = Random.Range(0, menuItems.Count);
        return menuItems[index];
    }

    public MenuItem GetItemByName(string name)
    {
        return menuItems.Find(x => x.foodName == name);
    }
}
