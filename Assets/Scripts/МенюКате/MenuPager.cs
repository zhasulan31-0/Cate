using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuPager : MonoBehaviour
{
    [Header("Настройки меню")]
    public CanvasGroup[] panels; // все панели меню с CanvasGroup
    public float fadeTime = 0.5f; // время плавного появления/исчезновения

    private int currentPage = 0;
    private bool isAnimating = false; // блокировка во время анимации

    private void Start()
    {
        // Скрываем все панели кроме первой
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == 0)
            {
                panels[i].alpha = 1;
                panels[i].interactable = true;
                panels[i].blocksRaycasts = true;
            }
            else
            {
                panels[i].alpha = 0;
                panels[i].interactable = false;
                panels[i].blocksRaycasts = false;
            }
        }
    }

    public void NextPage()
    {
        if (!isAnimating && currentPage < panels.Length - 1)
        {
            StartCoroutine(SwitchPanel(currentPage, currentPage + 1));
        }
    }

    public void PreviousPage()
    {
        if (!isAnimating && currentPage > 0)
        {
            StartCoroutine(SwitchPanel(currentPage, currentPage - 1));
        }
    }

    private IEnumerator SwitchPanel(int fromIndex, int toIndex)
    {
        isAnimating = true; // блокируем кнопки на время анимации

        CanvasGroup fromPanel = panels[fromIndex];
        CanvasGroup toPanel = panels[toIndex];

        // Включаем новую панель для плавного появления
        toPanel.interactable = true;
        toPanel.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime; // 🔹 используем unscaledDeltaTime
            float t = elapsed / fadeTime;

            fromPanel.alpha = Mathf.Lerp(1f, 0f, t);
            toPanel.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Финальная установка
        fromPanel.alpha = 0f;
        fromPanel.interactable = false;
        fromPanel.blocksRaycasts = false;

        toPanel.alpha = 1f;
        toPanel.interactable = true;
        toPanel.blocksRaycasts = true;

        currentPage = toIndex; // меняем страницу после анимации
        isAnimating = false;   // разблокируем кнопки
    }
}
