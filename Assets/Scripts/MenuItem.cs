using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MenuItem
{
    public string foodName;       // Название (например: "Капучино")
    public Sprite foodIcon;       // Иконка для заказа
    public GameObject prefab;     // Префаб для спавна еды
}

