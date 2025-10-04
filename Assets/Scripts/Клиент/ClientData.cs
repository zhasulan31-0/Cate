using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewClientData", menuName = "Cafe/ClientData")]
public class ClientData : ScriptableObject
{
    public string clientName;          // ��� ���� ������� ("�������", "VIP")
    public Sprite clientImage;         // �������
    public int minOrders = 1;          // ����������� ���������� ����
    public int maxOrders = 3;          // ������������ ���������� ����
    public float paymentMultiplier = 1f; // ��������� ������
    public List<FoodItemData> favoriteFoods; // ������� ����� �������� �� ����
}
