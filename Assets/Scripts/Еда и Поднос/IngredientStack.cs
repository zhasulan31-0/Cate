using UnityEngine;

public class InfiniteIngredientStack : MonoBehaviour
{
    [Header("Префаб ингредиента")]
    public GameObject foodPrefab;

    [Header("Данные ингредиента")]
    public FoodItemData foodData;

    [Header("Сколько сразу показываем для визуальной стопки")]
    public int startCount = 3;

    private void Start()
    {
        // Спавним стартовую стопку
        for (int i = 0; i < startCount; i++)
        {
            SpawnIngredient();
        }
    }

    /// <summary>
    /// Спавнит один ингредиент
    /// </summary>
    public FoodItem SpawnIngredient()
    {
        GameObject go = Instantiate(foodPrefab, transform);
        go.transform.localScale = Vector3.one;

        FoodItem item = go.GetComponent<FoodItem>();
        item.data = foodData;
        item.shelf = null; // не привязываем к полке

        // Добавляем компонент, чтобы при взятии ингредиента спавнился новый
        var taker = go.gameObject.AddComponent<IngredientTakenHandler>();
        taker.stack = this;

        // Случайная позиция для стопки
        Vector3 pos = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0f);
        go.transform.localPosition = pos;

        return item;
    }
}

/// <summary>
/// Обработчик, который спавнит новый ингредиент, когда игрок начал таскать текущий
/// </summary>
public class IngredientTakenHandler : MonoBehaviour, UnityEngine.EventSystems.IBeginDragHandler
{
    [HideInInspector] public InfiniteIngredientStack stack;

    public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // Спавним новый ингредиент сразу, как только игрок взял этот
        if (stack != null)
        {
            stack.SpawnIngredient();
        }

        // Удаляем этот компонент, чтобы он не спавнил снова
        Destroy(this);
    }
}
