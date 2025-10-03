using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip playSound;
    public AudioClip exitSound;

    public Color highlightColor = new Color(1f, 0.6f, 0f);
    private Color defaultColor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // не уничтожать при смене сцены
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayGame()
    {
        if (audioSource && playSound) audioSource.PlayOneShot(playSound);
        SceneManager.LoadSceneAsync(1);
    }

    public void ExitGame()
    {
        if (audioSource && exitSound) audioSource.PlayOneShot(exitSound);
        Application.Quit();
    }

    public void Hover(Button btn)
    {
        if (btn)
        {
            defaultColor = btn.image.color;
            btn.image.color = highlightColor;
        }
        if (audioSource && hoverSound) audioSource.PlayOneShot(hoverSound);
    }

    public void Unhover(Button btn)
    {
        if (btn) btn.image.color = defaultColor;
    }
}
