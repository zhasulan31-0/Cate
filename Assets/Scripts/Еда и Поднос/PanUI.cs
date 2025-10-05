using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class PanUI : MonoBehaviour, IDropHandler
{
    [System.Serializable]
    public class Recipe
    {
        public FoodItemData rawFood;     // сырой продукт
        public FoodItemData cookedFood;  // готовый продукт
        public float cookTime = 2f;      // время готовки
    }

    [Header("Рецепты (фильтр)")]
    public List<Recipe> recipes = new List<Recipe>();

    [Header("Зона ловли (точка, где лежит еда)")]
    public Transform catchZone;

    private FoodItem currentFood;
    private Recipe activeRecipe;
    private bool isCooking = false;

    private VerticalLayoutGroup layout;
    private Canvas canvas;

    private void Awake()
    {
        layout = GetComponent<VerticalLayoutGroup>();
        canvas = FindObjectOfType<Canvas>();
    }

    // Сковородка "ловит" еду при дропе
    public void OnDrop(PointerEventData eventData)
    {
        if (isCooking || currentFood != null)
            return; // занята

        var dropped = eventData.pointerDrag?.GetComponent<FoodItem>();
        if (dropped == null) return;

        TryCatchFood(dropped);
    }

    private void TryCatchFood(FoodItem food)
    {
        activeRecipe = recipes.Find(r => r.rawFood == food.data);
        if (activeRecipe == null) return;

        currentFood = food;

        // отвязка от полки/кофемашины
        if (food.shelf != null) { food.shelf.ReplaceFood(food); food.shelf = null; }
        if (food.isFromCoffeeMachine && food.coffeeMachine != null) { food.coffeeMachine.TakeCoffee(); food.isFromCoffeeMachine = false; food.coffeeMachine = null; }

        food.transform.SetParent(catchZone, false);
        food.transform.localPosition = Vector3.zero;
        food.enabled = false;
        layout.enabled = false;

        // ⚡ подписка на событие, когда еда попадёт на поднос
        food.onPlacedOnTray += OnFoodPlacedOnTray;

        StartCoroutine(CookProcess());
    }


    private IEnumerator CookProcess()
    {
        isCooking = true;
        yield return new WaitForSeconds(activeRecipe.cookTime);
        FinishCooking();
    }

    private void FinishCooking()
    {
        Debug.Log($"✅ {activeRecipe.rawFood.foodName} → {activeRecipe.cookedFood.foodName}");

        // удаляем сырое блюдо
        if (currentFood != null)
            Destroy(currentFood.gameObject);

        // создаём готовое прямо в зоне отлова
        GameObject cookedGO = Instantiate(activeRecipe.cookedFood.prefabUI, catchZone);
        cookedGO.transform.localPosition = Vector3.zero;

        FoodItem cookedItem = cookedGO.GetComponent<FoodItem>();
        cookedItem.data = activeRecipe.cookedFood;
        cookedItem.enabled = true;

        currentFood = cookedItem;
        isCooking = false;

        // захват включим, когда игрок заберёт блюдо
    }

    // Игрок забирает еду (например, при OnBeginDrag у FoodItem)
    private void OnFoodPlacedOnTray(FoodItem food)
    {
        if (food == currentFood)
        {
            Debug.Log("🍳 Еда поставлена на поднос — сковорода свободна!");
            currentFood = null;
            layout.enabled = true;

            // отписка, чтобы не было утечки событий
            food.onPlacedOnTray -= OnFoodPlacedOnTray;
        }
    }


}
