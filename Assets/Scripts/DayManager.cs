using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    [Header("��������� ���")]
    public int currentDay = 1;
    public int maxDay = 10;
    public int startClients = 10;
    public int endClients = 42;

    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 20f;
    public float spawnIntervalReductionPerDay = 1.5f;

    [Header("������")]
    public ClientSpawner spawner;
    public RestaurantMenuUI menuUI; // ������ �� UI ����

    private int clientsToSpawn;
    private int spawnedClients = 0;
    private int finishedClients = 0;

    private void Start()
    {
        StartDay(currentDay);
    }

    public void StartDay(int day)
    {
        currentDay = day;

        clientsToSpawn = Mathf.RoundToInt(Mathf.Lerp(startClients, endClients, (currentDay - 1f) / (maxDay - 1f)));
        spawnedClients = 0;
        finishedClients = 0;

        StartCoroutine(SpawnClientsRoutine());
        Debug.Log($"���� {currentDay} �������! ����� ��������: {clientsToSpawn}");
    }

    private IEnumerator SpawnClientsRoutine()
    {
        while (spawnedClients < clientsToSpawn)
        {
            spawner.SpawnClient();
            spawnedClients++;

            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
            interval = Mathf.Max(0.1f, interval - (currentDay - 1) * spawnIntervalReductionPerDay);

            yield return new WaitForSecondsRealtime(interval);
        }
    }

    public void NotifyClientFinished()
    {
        finishedClients++;

        if (finishedClients >= clientsToSpawn)
        {
            EndDay();
        }
    }

    private void EndDay()
    {
        Debug.Log($"���� {currentDay} ����������!");

        // ���� ��� �� ��������� � ���������� ���� �������� � ������
        if (currentDay < maxDay)
        {
            if (menuUI != null)
                menuUI.OpenMenu(); // ��������� ���� � ��������� � ������ ���� �� �����
        }
        else
        {
            // ��� ��� �������� � ����� � ������� �� MainMenu
            ResetProgress();
            SceneManager.LoadScene("MainMenu");
        }
    }

    // ���������� ������� "������� �������"
    public void OnCloseShop()
    {
        if (menuUI != null)
            menuUI.CloseMenu();

        if (currentDay < maxDay)
        {
            StartDay(currentDay + 1);
        }
        else
        {
            ResetProgress();
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void ResetProgress()
    {
        PlayerPrefs.DeleteKey("PlayerMoney");
        currentDay = 1;
    }
}
