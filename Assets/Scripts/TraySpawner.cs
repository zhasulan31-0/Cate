using UnityEngine;
using UnityEngine.UI;

public class TraySpawner : MonoBehaviour
{
    [Header("���������")]
    public GameObject trayPrefab;    // ������ �������
    public Transform spawnPoint;     // ����� ������
    public Button spawnButton;       // UI-������ (��������� � ����������)

    private void Awake()
    {
        // ����������� ������
        if (spawnButton != null)
            spawnButton.onClick.AddListener(SpawnTray);
    }

    private void OnDestroy()
    {
        // ����������� ������� ��������, ����� �� ���� ������
        if (spawnButton != null)
            spawnButton.onClick.RemoveListener(SpawnTray);
    }

    void SpawnTray()
    {
        if (trayPrefab != null && spawnPoint != null)
        {
            Instantiate(trayPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            Debug.Log("������ ���������!");
        }
    }
}
