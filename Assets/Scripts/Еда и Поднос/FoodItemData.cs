using UnityEngine;

[CreateAssetMenu(fileName = "NewFoodItem", menuName = "Cafe/FoodItemData")]
public class FoodItemData : ScriptableObject
{
    [Header("�������� ������")]
    public string foodName;
    public Sprite foodIcon;

    [Header("�������")]
    public GameObject prefabUI;   // ������ ��� UI/�������

    [Header("�������������")]
    public AudioClip eatSound;
    public int price = 10;
}
