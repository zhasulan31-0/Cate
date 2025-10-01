using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TrayUI : MonoBehaviour
{
    [Header("����� ������� (UI Image)")]
    public Image[] slots; // 3 ��������������� �����
    private List<Sprite> storedFoods = new List<Sprite>();

    public int maxSlots = 3;

    // ��������� ��� �� ������
    public bool AddFood(Sprite foodSprite)
    {
        if (storedFoods.Count >= maxSlots)
        {
            Debug.Log("������ �����!");
            return false;
        }

        storedFoods.Add(foodSprite);
        UpdateSlots();
        return true;
    }

    // ������� ��� �� �������
    public void RemoveFood(int index)
    {
        if (index < 0 || index >= storedFoods.Count) return;

        storedFoods.RemoveAt(index);
        UpdateSlots();
    }

    // ���������� UI-������
    private void UpdateSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < storedFoods.Count)
            {
                slots[i].sprite = storedFoods[i];
                slots[i].enabled = true;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].enabled = false;
            }
        }
    }

    public bool IsEmpty()
    {
        return storedFoods.Count == 0;
    }
}
