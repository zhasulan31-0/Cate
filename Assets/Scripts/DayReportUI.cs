using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DayReportUI : MonoBehaviour
{
    [Header("UI элементы")]
    public CanvasGroup reportCanvas;
    public TextMeshProUGUI dayNumberText;
    public TextMeshProUGUI totalClientsText;
    public TextMeshProUGUI happyClientsText;
    public TextMeshProUGUI moneyEarnedText;
    public Button goToShopButton;

    [Header("Настройки анимации")]
    public float fadeDuration = 0.4f;

    private void Awake()
    {
        if (reportCanvas != null)
            SetActive(false);

        if (goToShopButton != null)
            goToShopButton.onClick.AddListener(OnGoToShopClicked);
    }

    private void OnDestroy()
    {
        if (goToShopButton != null)
            goToShopButton.onClick.RemoveListener(OnGoToShopClicked);
    }

    public void ShowReport(int day, int totalClients, int happyClients, int unhappyClients, int moneyEarned)
    {
        if (reportCanvas == null) return;

        dayNumberText.text = $"День {day} завершён!";
        totalClientsText.text = $"Всего клиентов: {totalClients}";
        happyClientsText.text = $"Довольных: {happyClients}";
        moneyEarnedText.text = $"Заработано за день: $ {moneyEarned}";

        SetActive(true);
        StartCoroutine(FadeCanvas(reportCanvas, true));
    }

    private void SetActive(bool active)
    {
        reportCanvas.alpha = active ? 1f : 0f;
        reportCanvas.interactable = active;
        reportCanvas.blocksRaycasts = active;
        reportCanvas.gameObject.SetActive(active);
    }

    private IEnumerator FadeCanvas(CanvasGroup canvasGroup, bool fadeIn)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        float endAlpha = fadeIn ? 1f : 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
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
    }

    private void OnGoToShopClicked()
    {
        // Переходим в сцену магазина
        SceneManager.LoadScene("ShopScene"); // 🔹 замени на своё имя сцены магазина
    }
}
