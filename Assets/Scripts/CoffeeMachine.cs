using UnityEngine;
using System.Collections;

public class CoffeeMachine2D : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject coffeePrefab;    // Префаб готового кофе (UI элемент, например Image)
    public Transform spawnPoint;       // Точка появления (UI объект под Canvas)
    public float brewTime = 5f;        // Время приготовления (сек)

    private bool isBrewing = false;
    private bool hasCoffee = false;
    private GameObject currentCoffee;

    private void OnMouseDown()
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

        // Создаём кофе как UI элемент под Canvas
        currentCoffee = Instantiate(coffeePrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        currentCoffee.transform.localPosition = Vector3.zero; // по центру spawnPoint

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
