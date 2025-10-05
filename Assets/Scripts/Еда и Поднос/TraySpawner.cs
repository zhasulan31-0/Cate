using UnityEngine;
using UnityEngine.UI;

public class TraySpawner : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject trayPrefab;
    public Transform spawnPoint;
    public Button spawnButton;

    private TrayUI currentTray; // текущий поднос

    private void Awake()
    {
        if (spawnButton != null)
            spawnButton.onClick.AddListener(SpawnTray);
    }

    private void OnDestroy()
    {
        if (spawnButton != null)
            spawnButton.onClick.RemoveListener(SpawnTray);
    }

    void SpawnTray()
    {
        if (currentTray != null) // 🚫 если уже есть поднос, новый не спавним
        {
            Debug.Log("Поднос уже существует!");
            return;
        }

        if (trayPrefab != null && spawnPoint != null)
        {
            GameObject trayObj = Instantiate(trayPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            currentTray = trayObj.GetComponent<TrayUI>();

            if (currentTray != null)
            {
                // подписываемся на событие уничтожения
                currentTray.onTrayDestroyed += HandleTrayDestroyed;
            }

            Debug.Log("Поднос заспавнен!");
        }
    }

    void HandleTrayDestroyed()
    {
        currentTray = null; // освободили слот
        Debug.Log("Поднос уничтожен, можно спавнить новый.");
    }
}
