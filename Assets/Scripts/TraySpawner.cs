using UnityEngine;
using UnityEngine.UI;

public class TraySpawner : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject trayPrefab;    // Префаб подноса
    public Transform spawnPoint;     // Точка спавна
    public Button spawnButton;       // UI-кнопка (привязать в инспекторе)

    private void Awake()
    {
        // подписываем кнопку
        if (spawnButton != null)
            spawnButton.onClick.AddListener(SpawnTray);
    }

    private void OnDestroy()
    {
        // обязательно убираем подписку, чтобы не было утечек
        if (spawnButton != null)
            spawnButton.onClick.RemoveListener(SpawnTray);
    }

    void SpawnTray()
    {
        if (trayPrefab != null && spawnPoint != null)
        {
            Instantiate(trayPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            Debug.Log("Поднос заспавнен!");
        }
    }
}
