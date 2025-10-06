using UnityEngine;
using UnityEngine.UI;

public class QuitGameButton : MonoBehaviour
{
    [Header("Кнопка для выхода")]
    public Button quitButton;

    private void Start()
    {
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        else
            Debug.LogWarning("Quit button не назначена!");
    }

    private void QuitGame()
    {
        Debug.Log("Выход из игры...");

        // В редакторе Unity это не выйдет из игры, поэтому добавим условие
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
