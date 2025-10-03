using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoffeeMachineUI : MonoBehaviour
{
    [Header("UI Настройки")]
    public Button machineButton;       // Кнопка на картинке автомата
    public GameObject coffeePrefab;    // Префаб готового кофе (UI Image)
    public Transform spawnPoint;       // UI точка появления (например, пустой GameObject в Canvas)
    public float brewTime = 5f;        // Время приготовления (сек)

    private bool isBrewing = false;
    private bool hasCoffee = false;
    private GameObject currentCoffee;

    private void Awake()
    {
        if (machineButton != null)
        {
            machineButton.onClick.AddListener(OnMachineClick);
        }
    }

    private void OnDestroy()
    {
        if (machineButton != null)
        {
            machineButton.onClick.RemoveListener(OnMachineClick);
        }
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

        // Спавним UI-кофе
        currentCoffee = Instantiate(coffeePrefab, spawnPoint);
        currentCoffee.transform.localPosition = Vector3.zero;

        hasCoffee = true;
        isBrewing = false;

        Debug.Log("Кофемашина: Кофе готов!");
    }

    public void TakeCoffee()
    {
        if (hasCoffee && currentCoffee != null)
        {
            Destroy(currentCoffee);
            hasCoffee = false;
            Debug.Log("Кофемашина: Кофе забран!");
        }
    }
}
