using UnityEngine;
using UnityEngine.UI;

public class QuitGameButton : MonoBehaviour
{
    [Header("������ ��� ������")]
    public Button quitButton;

    private void Start()
    {
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        else
            Debug.LogWarning("Quit button �� ���������!");
    }

    private void QuitGame()
    {
        Debug.Log("����� �� ����...");

        // � ��������� Unity ��� �� ������ �� ����, ������� ������� �������
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
