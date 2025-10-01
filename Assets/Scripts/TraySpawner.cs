using UnityEngine;

public class TraySpawner : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject trayPrefab;    // Префаб подноса
    public Transform spawnPoint;     // Точка спавна

    private bool isMouseInside = false;  // Флаг, находится ли мышь в зоне стопки

    private void OnMouseEnter()
    {
        isMouseInside = true;
    }

    private void OnMouseExit()
    {
        isMouseInside = false;
    }

    void Update()
    {
        if (isMouseInside && Input.GetMouseButtonDown(0))
        {
            SpawnTray();
        }
    }

    void SpawnTray()
    {
        Instantiate(trayPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
    }
}
