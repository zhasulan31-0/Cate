using UnityEngine;
using TMPro;
using System.Collections;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance;

    [Header("UI")]
    public TextMeshProUGUI walletText;

    [Header("Анимация")]
    public float animateDuration = 0.5f;

    private Coroutine animateCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Переназначаем UI на новый компонент из сцены
            Instance.walletText = walletText;
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUIImmediate(UpgradeManager.Instance.GetMoney());
    }

    private void OnEnable()
    {
        if (Instance != null)
            UpdateUIImmediate(UpgradeManager.Instance.GetMoney());
    }


    public void UpdateUIAnimated(int targetMoney)
    {
        if (animateCoroutine != null)
            StopCoroutine(animateCoroutine);
        animateCoroutine = StartCoroutine(AnimateMoneyDisplay(targetMoney));
    }

    private IEnumerator AnimateMoneyDisplay(int targetMoney)
    {
        int from = 0;
        if (!string.IsNullOrEmpty(walletText.text))
            int.TryParse(walletText.text.Replace("$ ", ""), out from);

        float elapsed = 0f;
        while (elapsed < animateDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animateDuration);
            int displayValue = Mathf.RoundToInt(Mathf.Lerp(from, targetMoney, t));
            walletText.text = $"$ {displayValue}";
            yield return null;
        }

        walletText.text = $"$ {targetMoney}";
    }

    public void UpdateUIImmediate(int targetMoney)
    {
        walletText.text = $"$ {targetMoney}";
    }
}
