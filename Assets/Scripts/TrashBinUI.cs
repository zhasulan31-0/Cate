using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        // Проверяем, еда ли это
        FoodItem food = eventData.pointerDrag.GetComponent<FoodItem>();
        if (food != null)
        {
            TrashFood(food);
            return;
        }

        // Проверяем, поднос ли это
        TrayUI tray = eventData.pointerDrag.GetComponent<TrayUI>();
        if (tray != null)
        {
            TrashTray(tray);
        }
    }

    public void TrashFood(FoodItem food)
    {
        Debug.Log($"Еда {food.GetName()} выброшена в мусорку!");
        if (food.isFromCoffeeMachine && food.coffeeMachine != null)
        {
            food.coffeeMachine.TakeCoffee();
        }
        Destroy(food.gameObject);
    }

    public void TrashTray(TrayUI tray)
    {
        Debug.Log("Поднос выброшен в мусорку вместе с едой!");
        tray.DestroyTray();
    }
}
