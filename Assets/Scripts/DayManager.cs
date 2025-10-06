using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Настройки дня")]
    public int currentDay = 1;
    public int maxDay = 10;
    public int startClients = 10;
    public int endClients = 42;

    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 20f;
    public float spawnIntervalReductionPerDay = 1.5f;

    [Header("Ссылки")]
    public ClientSpawner spawner;
    public DayReportUI reportUI;

    private int clientsToSpawn;
    private int spawnedClients;
    private int finishedClients;
    private int happyClients;
    private int unhappyClients;
    private int totalEarned;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            // Обновляем ссылки на объекты сцены
            spawner = FindObjectOfType<ClientSpawner>();
            reportUI = FindObjectOfType<DayReportUI>();

            if (reportUI != null)
                reportUI.gameObject.SetActive(false);

            currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
            StartDay(currentDay);
        }
    }

    public void StartDay(int day)
    {
        currentDay = Mathf.Clamp(day, 1, maxDay);
        PlayerPrefs.SetInt("CurrentDay", currentDay);
        PlayerPrefs.Save();

        clientsToSpawn = Mathf.RoundToInt(Mathf.Lerp(startClients, endClients, (currentDay - 1f) / (maxDay - 1f)));
        spawnedClients = 0;
        finishedClients = 0;
        happyClients = 0;
        unhappyClients = 0;
        totalEarned = 0;

        StartCoroutine(SpawnClientsRoutine());
        Debug.Log($"📅 День {currentDay} начался! Клиентов сегодня: {clientsToSpawn}");
    }

    private IEnumerator SpawnClientsRoutine()
    {
        while (spawnedClients < clientsToSpawn)
        {
            if (spawner != null)
                spawner.SpawnClient();

            spawnedClients++;

            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
            interval = Mathf.Max(0.1f, interval - (currentDay - 1) * spawnIntervalReductionPerDay);

            yield return new WaitForSecondsRealtime(interval);
        }
    }

    public void NotifyClientFinished(bool wasHappy, int payment = 0)
    {
        finishedClients++;

        if (wasHappy)
        {
            happyClients++;
            totalEarned += payment;
        }
        else
        {
            unhappyClients++;
        }

        if (finishedClients >= clientsToSpawn)
            EndDay();
    }

    private void EndDay()
    {
        Debug.Log($"✅ День {currentDay} закончен!");
        Debug.Log($"Довольных: {happyClients} | Недовольных: {unhappyClients} | Заработано: {totalEarned}");

        if (reportUI != null)
            reportUI.ShowReport(currentDay, clientsToSpawn, happyClients, unhappyClients, totalEarned);
    }

    public void PrepareNextDay()
    {
        int nextDay = currentDay + 1;

        if (nextDay > maxDay)
        {
            Debug.Log("🏁 Все дни завершены! Сброс прогресса и возврат в главное меню...");
            UpgradeManager.Instance?.ResetProgress();

            PlayerPrefs.DeleteKey("CurrentDay");
            currentDay = 1;

            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            PlayerPrefs.SetInt("CurrentDay", nextDay);
            PlayerPrefs.Save();

            Debug.Log($"▶️ Переход к следующему дню ({nextDay})");
            SceneManager.LoadScene("SampleScene");
        }
    }
}
