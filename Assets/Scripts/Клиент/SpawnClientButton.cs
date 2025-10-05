using UnityEngine;
using UnityEngine.UI;

public class SpawnClientButton : MonoBehaviour
{
    [Header("������ �� �������")]
    public ClientSpawner spawner;

    private void Awake()
    {
        // �������� ��������� Button �� ���� �������
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("�� ������� ��� ���������� Button!");
        }
    }

    private void OnButtonClicked()
    {
        if (spawner != null)
        {
            spawner.SpawnClient();
        }
        else
        {
            Debug.LogError("Spawner �� ��������!");
        }
    }
}
