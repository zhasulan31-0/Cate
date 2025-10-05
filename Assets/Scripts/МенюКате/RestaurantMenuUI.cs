using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RestaurantMenuUI : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public Button menuButton;
    public CanvasGroup menuCanvas;
    public Button closeMenuButton;

    [Header("Настройки анимации")]
    public float fadeDuration = 0.3f;

    private bool isMenuOpen = false;

    private void Awake()
    {
        if (menuButton != null)
            menuButton.onClick.AddListener(OpenMenu);

        if (closeMenuButton != null)
            closeMenuButton.onClick.AddListener(CloseMenu);

        SetCanvasActive(menuCanvas, false);
    }

    private void OnDestroy()
    {
        if (menuButton != null)
            menuButton.onClick.RemoveListener(OpenMenu);

        if (closeMenuButton != null)
            closeMenuButton.onClick.RemoveListener(CloseMenu);
    }

    public void OpenMenu()
    {
        if (menuCanvas == null || isMenuOpen) return;

        isMenuOpen = true;

        // Ставим игру на паузу
        Time.timeScale = 0f;

        menuCanvas.gameObject.SetActive(true);
        StartCoroutine(FadeCanvas(menuCanvas, true));
    }

    public void CloseMenu()
    {
        if (menuCanvas == null || !isMenuOpen) return;

        isMenuOpen = false;

        StartCoroutine(FadeCanvas(menuCanvas, false, () =>
        {
            // Возобновляем игру
            Time.timeScale = 1f;
        }));
    }

    private IEnumerator FadeCanvas(CanvasGroup canvasGroup, bool fadeIn, System.Action onComplete = null)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        float endAlpha = fadeIn ? 1f : 0f;

        if (fadeIn)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // работает даже при паузе
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (!fadeIn)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(false);
        }

        onComplete?.Invoke();
    }

    private void SetCanvasActive(CanvasGroup canvasGroup, bool active)
    {
        canvasGroup.alpha = active ? 1f : 0f;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
        canvasGroup.gameObject.SetActive(active);
    }
}
