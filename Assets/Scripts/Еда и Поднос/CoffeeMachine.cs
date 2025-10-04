using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoffeeMachineUI : MonoBehaviour
{
    [Header("UI Настройки")]
    public Button machineButton;
    public GameObject coffeePrefab;
    public Transform spawnPoint;
    public float brewTime = 5f;

    private bool isBrewing = false;
    private bool hasCoffee = false;
    private GameObject currentCoffee;

    private void Awake()
    {
        if (machineButton != null)
            machineButton.onClick.AddListener(OnMachineClick);
    }

    private void OnDestroy()
    {
        if (machineButton != null)
            machineButton.onClick.RemoveListener(OnMachineClick);
    }

    private void OnMachineClick()
    {
        if (!isBrewing && !hasCoffee)
        {
            StartCoroutine(BrewCoffee());
        }
        else if (isBrewing)
        {
            Debug.Log("Кофемашина уже варит кофе!");
        }
        else if (hasCoffee)
        {
            Debug.Log("Забери готовое кофе перед новым заказом!");
        }
    }

    private IEnumerator BrewCoffee()
    {
        isBrewing = true;
        Debug.Log("Кофемашина: Начало приготовления...");

        float remainingTime = brewTime;
        while (remainingTime > 0)
        {
            Debug.Log("Осталось: " + remainingTime + " сек.");
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        // Спавним кофе
        currentCoffee = Instantiate(coffeePrefab, spawnPoint);
        currentCoffee.transform.localPosition = Vector3.zero;

        // сообщаем, что это кофе с автомата
        FoodItem fi = currentCoffee.GetComponent<FoodItem>();
        fi.isFromCoffeeMachine = true;
        fi.coffeeMachine = this;

        hasCoffee = true;
        isBrewing = false;

        Debug.Log("Кофемашина: Кофе готов!");
    }

    // Вызывается только когда кофе реально попал на поднос
    public void TakeCoffee()
    {
        if (hasCoffee && currentCoffee != null)
        {
            hasCoffee = false;
            currentCoffee = null;
            Debug.Log("Кофемашина: Кофе успешно забран на поднос!");
        }
    }

    // Возвращаем кофе на автомат, если игрок бросил мимо
    public void ReturnCoffee(FoodItem coffee)
    {
        coffee.transform.SetParent(spawnPoint, false);
        coffee.transform.localPosition = Vector3.zero;
        coffee.transform.localScale = Vector3.one;
        Debug.Log("Кофе возвращено на автомат.");
    }
}
