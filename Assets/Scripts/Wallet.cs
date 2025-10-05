using UnityEngine;
using TMPro;
using System.Collections;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance;

    [Header("UI")]
    public TextMeshProUGUI walletText;

    [Header("Сохранение")]
    public string saveKey = "PlayerMoney";

    [Header("Анимация")]
    public float animateDuration = 0.5f; // скорость анимации

    private int money = 0;
    private Coroutine animateCoroutine;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadMoney();
        UpdateUIImmediate();
    }

    private void LoadMoney()
    {
        money = PlayerPrefs.GetInt(saveKey, 0);
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt(saveKey, money);
        PlayerPrefs.Save();
    }

    public int GetMoney() => money;

    public void AddMoney(int amount)
    {
        int oldMoney = money;
        money += amount;
        SaveMoney();

        if (animateCoroutine != null)
            StopCoroutine(animateCoroutine);
        animateCoroutine = StartCoroutine(AnimateMoney(oldMoney, money));

        Debug.Log($"Заработано: {amount}. Всего в кошельке: {money}");
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            int oldMoney = money;
            money -= amount;
            SaveMoney();

            if (animateCoroutine != null)
                StopCoroutine(animateCoroutine);
            animateCoroutine = StartCoroutine(AnimateMoney(oldMoney, money));

            return true;
        }
        return false;
    }

    private IEnumerator AnimateMoney(int from, int to)
    {
        float elapsed = 0f;
        while (elapsed < animateDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animateDuration);
            int displayValue = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            walletText.text = $"$ {displayValue}";
            yield return null;
        }

        walletText.text = $"$ {to}";
    }

    private void UpdateUIImmediate()
    {
        if (walletText != null)
            walletText.text = $"$ {money}";
    }
}
