using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClientData", menuName = "Cafe/ClientData")]
public class ClientData : ScriptableObject
{
    public string clientName;
    public Sprite clientImage;
    public int minOrders = 1;
    public int maxOrders = 3;
    public float paymentMultiplier = 1f;
    public List<FoodItemData> favoriteFoods;
    public string happyAnimationName = "Happy";
    public AudioClip happySound;

    [Header("Терпение")]
    public float patienceTime = 10f; // сколько секунд клиент готов ждать
}
