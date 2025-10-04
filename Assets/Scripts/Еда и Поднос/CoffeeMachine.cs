using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoffeeMachineUI : MonoBehaviour
{
    [Header("UI ���������")]
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
            Debug.Log("���������� ��� ����� ����!");
        }
        else if (hasCoffee)
        {
            Debug.Log("������ ������� ���� ����� ����� �������!");
        }
    }

    private IEnumerator BrewCoffee()
    {
        isBrewing = true;
        Debug.Log("����������: ������ �������������...");

        float remainingTime = brewTime;
        while (remainingTime > 0)
        {
            Debug.Log("��������: " + remainingTime + " ���.");
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        // ������� ����
        currentCoffee = Instantiate(coffeePrefab, spawnPoint);
        currentCoffee.transform.localPosition = Vector3.zero;

        // ��������, ��� ��� ���� � ��������
        FoodItem fi = currentCoffee.GetComponent<FoodItem>();
        fi.isFromCoffeeMachine = true;
        fi.coffeeMachine = this;

        hasCoffee = true;
        isBrewing = false;

        Debug.Log("����������: ���� �����!");
    }

    // ���������� ������ ����� ���� ������� ����� �� ������
    public void TakeCoffee()
    {
        if (hasCoffee && currentCoffee != null)
        {
            hasCoffee = false;
            currentCoffee = null;
            Debug.Log("����������: ���� ������� ������ �� ������!");
        }
    }

    // ���������� ���� �� �������, ���� ����� ������ ����
    public void ReturnCoffee(FoodItem coffee)
    {
        coffee.transform.SetParent(spawnPoint, false);
        coffee.transform.localPosition = Vector3.zero;
        coffee.transform.localScale = Vector3.one;
        Debug.Log("���� ���������� �� �������.");
    }
}
