using UnityEngine;

public class InfiniteIngredientStack : MonoBehaviour
{
    [Header("������ �����������")]
    public GameObject foodPrefab;

    [Header("������ �����������")]
    public FoodItemData foodData;

    [Header("������� ����� ���������� ��� ���������� ������")]
    public int startCount = 3;

    private void Start()
    {
        // ������� ��������� ������
        for (int i = 0; i < startCount; i++)
        {
            SpawnIngredient();
        }
    }

    /// <summary>
    /// ������� ���� ����������
    /// </summary>
    public FoodItem SpawnIngredient()
    {
        GameObject go = Instantiate(foodPrefab, transform);
        go.transform.localScale = Vector3.one;

        FoodItem item = go.GetComponent<FoodItem>();
        item.data = foodData;
        item.shelf = null; // �� ����������� � �����

        // ��������� ���������, ����� ��� ������ ����������� ��������� �����
        var taker = go.gameObject.AddComponent<IngredientTakenHandler>();
        taker.stack = this;

        // ��������� ������� ��� ������
        Vector3 pos = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0f);
        go.transform.localPosition = pos;

        return item;
    }
}

/// <summary>
/// ����������, ������� ������� ����� ����������, ����� ����� ����� ������� �������
/// </summary>
public class IngredientTakenHandler : MonoBehaviour, UnityEngine.EventSystems.IBeginDragHandler
{
    [HideInInspector] public InfiniteIngredientStack stack;

    public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // ������� ����� ���������� �����, ��� ������ ����� ���� ����
        if (stack != null)
        {
            stack.SpawnIngredient();
        }

        // ������� ���� ���������, ����� �� �� ������� �����
        Destroy(this);
    }
}
