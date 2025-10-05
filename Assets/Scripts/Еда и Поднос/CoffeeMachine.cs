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

    [Header("Прогресс бар")]
    public Image progressBar; // ссылка на UI Image

    private bool isBrewing = false;
    private bool hasCoffee = false;
    private GameObject currentCoffee;

    [Header("Настройки данных")]
    public FoodItemData coffeeData;

    private void Awake()
    {
        if (machineButton != null)
            machineButton.onClick.AddListener(OnMachineClick);

        if (progressBar != null)
            progressBar.fillAmount = 0f; // стартовое значение
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

        float elapsed = 0f;

        // показываем прогресс бар
        if (progressBar != null)
            progressBar.gameObject.SetActive(true);

        while (elapsed < brewTime)
        {
            elapsed += Time.deltaTime;
            if (progressBar != null)
                progressBar.fillAmount = Mathf.Clamp01(elapsed / brewTime);

            yield return null;
        }

        // спавним кофе
        currentCoffee = Instantiate(coffeePrefab, spawnPoint);
        currentCoffee.transform.localPosition = Vector3.zero;

        FoodItem fi = currentCoffee.GetComponent<FoodItem>();
        fi.isFromCoffeeMachine = true;
        fi.coffeeMachine = this;
        fi.data = coffeeData;

        hasCoffee = true;
        isBrewing = false;

        Debug.Log("Кофемашина: Кофе готов!");

        // скрываем прогресс бар
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }

    public void TakeCoffee()
    {
        if (hasCoffee && currentCoffee != null)
        {
            hasCoffee = false;
            currentCoffee = null;
            Debug.Log("Кофемашина: Кофе успешно забран на поднос!");
        }
    }

    public void ReturnCoffee(FoodItem coffee)
    {
        coffee.transform.SetParent(spawnPoint, false);
        coffee.transform.localPosition = Vector3.zero;
        coffee.transform.localScale = Vector3.one;
        Debug.Log("Кофе возвращено на автомат.");
    }
}
