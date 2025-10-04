using UnityEngine;
using UnityEngine.UI;

public class SpawnClientButton : MonoBehaviour
{
    [Header("Ссылка на спавнер")]
    public ClientSpawner spawner;

    private void Awake()
    {
        // Получаем компонент Button на этом объекте
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("На объекте нет компонента Button!");
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
            Debug.LogError("Spawner не назначен!");
        }
    }
}
