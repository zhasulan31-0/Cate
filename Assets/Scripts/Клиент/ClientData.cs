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

    [Header("��������")]
    public float patienceTime = 10f; // ������� ������ ������ ����� �����

    // --- ���������� ������������ �������� ---
    [HideInInspector] public float defaultPatienceTime;
    [HideInInspector] public float defaultPaymentMultiplier;

    private void OnEnable()
    {
        // ��������� �������� ������ ���� ��� ��� �� �������������
        if (defaultPatienceTime == 0) defaultPatienceTime = patienceTime;
        if (defaultPaymentMultiplier == 0) defaultPaymentMultiplier = paymentMultiplier;
    }

    public void ResetToDefault()
    {
        patienceTime = defaultPatienceTime;
        paymentMultiplier = defaultPaymentMultiplier;
    }
}
