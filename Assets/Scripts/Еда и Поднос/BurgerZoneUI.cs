using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class BurgerZoneUI : MonoBehaviour, IDropHandler
{
    [Header("Допустимая последовательность (по шагам)")]
    public List<FoodItemData> allowedSequence; // [низ -> верх]

    [Header("Зона сборки бургера (где слои кладутся)")]
    public RectTransform stackZone;

    [Header("Вертикальное смещение между слоями (в локальных единицах RectTransform)")]
    public float layerHeight = 30f;

    [Header("Финальный готовый бургер (если нужно создать)")]
    public FoodItemData finalBurgerData;

    private List<FoodItem> stackedFoods = new List<FoodItem>();
    private Canvas canvas;
    private VerticalLayoutGroup layoutGroup;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        layoutGroup = GetComponent<VerticalLayoutGroup>();

        // Убедимся, что есть Graphic для получения событий (OnDrop)
        if (GetComponent<UnityEngine.UI.Graphic>() == null)
        {
            var img = gameObject.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(1f, 1f, 1f, 0f); // прозрачная область, но RaycastTarget = true
        }

        if (stackZone == null)
            stackZone = transform as RectTransform;
    }

    // UI drop
    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<FoodItem>();
        if (dragged == null) return;

        TryAddIngredient(dragged);
    }

    // Публичный метод — чтобы FoodItem мог вызвать напрямую при OnEndDrag
    public bool TryAddIngredient(FoodItem food)
    {
        int nextIndex = stackedFoods.Count;

        // Проверка завершения
        if (nextIndex >= allowedSequence.Count)
        {
            Debug.Log("❌ Бургер уже собран полностью!");
            return false;
        }

        var expected = allowedSequence[nextIndex];
        if (food.data != expected)
        {
            Debug.Log($"❌ Ожидался {expected.foodName}, а положили {food.data.foodName}");
            return false;
        }

        // success — кладём слой
        Debug.Log($"✅ Добавлен слой: {food.data.foodName}");

        // 🔹 отвязываем источник — если еда была с полки, она респавнится
        food.shelf = null;

        // ставим в stackZone и позиционируем ровно
        RectTransform foodRT = food.GetComponent<RectTransform>();
        foodRT.SetParent(stackZone, false);

        Vector2 anchored = Vector2.zero;
        anchored.y = nextIndex * layerHeight;
        foodRT.anchoredPosition = anchored;

        // блокируем перетаскивание слоя
        food.enabled = false;
        var cg = food.GetComponent<CanvasGroup>();
        if (cg == null) cg = food.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = true;

        stackedFoods.Add(food);

        // если завершили — создаём готовый бургер
        if (stackedFoods.Count == allowedSequence.Count)
            OnBurgerCompleted();

        return true;
    }


    private void OnBurgerCompleted()
    {
        Debug.Log("🍔 Бургер полностью собран!");

        // удаляем слои (всё, что было)
        foreach (var f in stackedFoods)
        {
            if (f != null)
                Destroy(f.gameObject);
        }
        stackedFoods.Clear();

        // создаём готовый бургер в зоне
        if (finalBurgerData != null)
        {
            GameObject readyGO = Instantiate(finalBurgerData.prefabUI, stackZone);
            var rt = readyGO.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;

            FoodItem burger = readyGO.GetComponent<FoodItem>();
            burger.data = finalBurgerData;

            // убедимся, что у готового бургера есть CanvasGroup
            var cg = burger.GetComponent<CanvasGroup>();
            if (cg == null) cg = burger.gameObject.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = true;

            burger.enabled = true; // сделать его можно таскать
        }
    }

    // Вызываем, когда игрок начала таскать предмет, который был в нашей зоне (например, готовый бургер)
    public void OnBurgerTaken(FoodItem taken)
    {
        // если это финальный бургер — освобождаем зону
        if (finalBurgerData != null && taken.data == finalBurgerData)
        {
            Debug.Log("🍔 Готовый бургер снят — зона освобождена.");
            stackedFoods.Clear();
            // можно очистить дочерние если что-то осталось
            // включим возможность собирать новый бургер (ничего дополнительно делать не нужно)
        }
        else
        {
            // если игрок снял промежуточный слой (не рекомендуется), попытаемся удалить его из списка
            if (stackedFoods.Contains(taken))
            {
                stackedFoods.Remove(taken);
                Debug.Log("Слой бургер-зоны удалён вручную.");
            }
        }
    }
}
