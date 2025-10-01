using UnityEngine;
using System.Collections;

public class CoffeeMachine2D : MonoBehaviour
{
    [Header("���������")]
    public GameObject coffeePrefab;    // ������ �������� ���� (UI �������, �������� Image)
    public Transform spawnPoint;       // ����� ��������� (UI ������ ��� Canvas)
    public float brewTime = 5f;        // ����� ������������� (���)

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

        // ������ ���� ��� UI ������� ��� Canvas
        currentCoffee = Instantiate(coffeePrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        currentCoffee.transform.localPosition = Vector3.zero; // �� ������ spawnPoint

        hasCoffee = true;
        isBrewing = false;

        Debug.Log("����������: ���� �����!");
    }

    public void TakeCoffee()
    {
        if (hasCoffee && currentCoffee != null)
        {
            Destroy(currentCoffee);
            hasCoffee = false;
            Debug.Log("����������: ���� ������!");
        }
    }
}
