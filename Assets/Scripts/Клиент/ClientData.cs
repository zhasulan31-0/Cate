using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewClientData", menuName = "Cafe/ClientData")]
public class ClientData : ScriptableObject
{
    public string clientName;          // Имя типа клиента ("Обычный", "VIP")
    public Sprite clientImage;         // Портрет
    public int minOrders = 1;          // Минимальное количество блюд
    public int maxOrders = 3;          // Максимальное количество блюд
    public float paymentMultiplier = 1f; // Множитель оплаты
    public List<FoodItemData> favoriteFoods; // Любимые блюда напрямую из меню
}
