using UnityEngine;

public class TraySpawner : MonoBehaviour
{
    [Header("���������")]
    public GameObject trayPrefab;    // ������ �������
    public Transform spawnPoint;     // ����� ������

    private bool isMouseInside = false;  // ����, ��������� �� ���� � ���� ������

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
