using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UpgradeShop : MonoBehaviour
{
    [Header("UI элементов")]
    public TextMeshProUGUI foodUpgradeInfoText;
    public TextMeshProUGUI clientUpgradeInfoText;
    public TextMeshProUGUI cookSpeedMaxText;
    public TextMeshProUGUI dayText;

    [Header("Тексты цен апгрейдов")]
    public TextMeshProUGUI patiencePriceText;
    public TextMeshProUGUI cookSpeedPriceText;
    public TextMeshProUGUI randomClientPriceText;
    public TextMeshProUGUI randomFoodPriceText;

    [Header("Кнопки апгрейдов")]
    public Button patienceButton;
    public Button cookSpeedButton;
    public Button randomClientButton;
    public Button randomFoodButton;
    public Button startDayButton;

    private UpgradeManager upgradeManager;

    private void Start()
    {
        upgradeManager = UpgradeManager.Instance;

        if (upgradeManager == null)
        {
            Debug.LogError("❌ UpgradeManager не найден на сцене!");
            return;
        }

        // Подписка на кнопки
        patienceButton?.onClick.AddListener(() => OnBuyUpgrade(0));
        cookSpeedButton?.onClick.AddListener(() => OnBuyUpgrade(1));
        randomClientButton?.onClick.AddListener(() => OnBuyUpgrade(2));
        randomFoodButton?.onClick.AddListener(() => OnBuyUpgrade(3));
        startDayButton?.onClick.AddListener(OnStartDay);

        UpdateUI();
        UpdateDayText();
    }

    private void OnBuyUpgrade(int type)
    {
        upgradeManager.BuyUpgrade(type);
        UpdateUI();
    }

    private void OnStartDay()
    {
        int current = PlayerPrefs.GetInt("CurrentDay", 1);
        int next = current + 1;
        PlayerPrefs.SetInt("CurrentDay", next);
        PlayerPrefs.Save();

        if (DayManager.Instance != null)
            DayManager.Instance.PrepareNextDay();
        else
            SceneManager.LoadScene("SampleScene");
    }

    private void UpdateUI()
    {
        if (upgradeManager == null) return;


        patiencePriceText.text = $"{upgradeManager.GetUpgradeCost(upgradeManager.basePatienceCost, upgradeManager.patienceLevel)}$";
        cookSpeedPriceText.text = $"{upgradeManager.GetUpgradeCost(upgradeManager.baseCookSpeedCost, upgradeManager.cookSpeedLevel)}$";
        randomClientPriceText.text = $"{upgradeManager.GetUpgradeCost(upgradeManager.baseRandomClientCost, upgradeManager.randomClientMultLevel)}$";
        randomFoodPriceText.text = $"{upgradeManager.GetUpgradeCost(upgradeManager.baseRandomFoodCost, upgradeManager.randomFoodPriceLevel)}$";

        foodUpgradeInfoText.text = string.IsNullOrEmpty(upgradeManager.lastUpgradedFoodName)
            ? " "
            : $"{upgradeManager.lastUpgradedFoodName}: {upgradeManager.lastFoodOldPrice}$ → {upgradeManager.lastUpgradedFoodPrice}$";

        clientUpgradeInfoText.text = string.IsNullOrEmpty(upgradeManager.lastClientName)
            ? " "
            : $"{upgradeManager.lastClientName}: x{upgradeManager.lastClientOldMult:F2} → x{upgradeManager.lastClientNewMult:F2}";

        cookSpeedMaxText?.gameObject.SetActive(upgradeManager.cookSpeedMaxReached);
    }

    private void UpdateDayText()
    {
        int day = PlayerPrefs.GetInt("CurrentDay", 1);
        dayText.text = $"День {day}";
    }
}
