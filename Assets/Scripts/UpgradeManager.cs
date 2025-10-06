using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Баланс игрока")]
    public int money = 0;

    [Header("Прокачки уровней")]
    public int patienceLevel = 0;
    public int cookSpeedLevel = 0;
    public int randomClientMultLevel = 0;
    public int randomFoodPriceLevel = 0;

    [Header("Параметры прогрессии")]
    public float patienceBonusPerLevel = 0.5f;
    public float cookSpeedReducePerLevel = 0.1f;
    public float minPanTime = 0.6f;
    public float minCoffeeTime = 0.6f;
    public float randomClientBonus = 0.05f;
    public float randomFoodBonusPercent = 10f;

    [Header("Базовые стоимости улучшений")]
    public int basePatienceCost = 50;
    public int baseCookSpeedCost = 75;
    public int baseRandomClientCost = 100;
    public int baseRandomFoodCost = 100;

    [Header("Множитель роста цены апгрейда")]
    public float upgradePriceMultiplier = 1.5f;

    [Header("Последние улучшения")]
    public string lastUpgradedFoodName;
    public int lastUpgradedFoodPrice;
    public int lastFoodOldPrice;

    public string lastClientName;
    public float lastClientOldMult;
    public float lastClientNewMult;

    [Header("Флаг максимальной скорости готовки")]
    public bool cookSpeedMaxReached = false;

    private const string MoneyKey = "PlayerMoney";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ------------------ ДЕНЬГИ ------------------
    public int GetMoney() => money;

    public void AddMoney(int amount)
    {
        money += amount;
        SaveProgress();
        Wallet.Instance?.UpdateUIAnimated(money);
    }

    public bool SpendMoney(int amount)
    {
        if (money < amount) return false;
        money -= amount;
        SaveProgress();
        Wallet.Instance?.UpdateUIAnimated(money);
        return true;
    }

    // ------------------ ПРОКАЧКА ------------------
    public void UpgradePatience()
    {
        int cost = GetUpgradeCost(basePatienceCost, patienceLevel);
        if (!SpendMoney(cost)) return;

        patienceLevel++;
        ApplyPatienceUpgrade();
        SaveProgress();
    }

    public void UpgradeCookSpeed()
    {
        if (cookSpeedMaxReached) return;

        int cost = GetUpgradeCost(baseCookSpeedCost, cookSpeedLevel);
        if (!SpendMoney(cost)) return;

        cookSpeedLevel++;
        ApplyCookSpeedUpgrade();
        SaveProgress();
    }

    public void UpgradeRandomClient()
    {
        int cost = GetUpgradeCost(baseRandomClientCost, randomClientMultLevel);
        if (!SpendMoney(cost)) return;

        randomClientMultLevel++;
        ApplyRandomClientUpgrade();
        SaveProgress();
    }

    public void UpgradeRandomFoodPrice()
    {
        int cost = GetUpgradeCost(baseRandomFoodCost, randomFoodPriceLevel);
        if (!SpendMoney(cost)) return;

        randomFoodPriceLevel++;
        ApplyRandomFoodPriceUpgrade();
        SaveProgress();
    }

    public int GetUpgradeCost(int baseCost, int level)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(upgradePriceMultiplier, level));
    }

    // ------------------ ПРИМЕНЕНИЕ ------------------
    private void ApplyPatienceUpgrade()
    {
        foreach (var client in ClientDatabase.Instance.clientTypes)
            client.patienceTime += patienceBonusPerLevel;
    }

    private void ApplyCookSpeedUpgrade()
    {
        var pans = FindObjectsOfType<PanUI>();
        var coffees = FindObjectsOfType<CoffeeMachineUI>();
        bool reachedLimit = true;

        foreach (var pan in pans)
        {
            foreach (var recipe in pan.recipes)
            {
                float newTime = Mathf.Max(minPanTime, recipe.cookTime * (1f - cookSpeedReducePerLevel));
                if (newTime > minPanTime) reachedLimit = false;
                recipe.cookTime = newTime;
            }
        }

        foreach (var coffee in coffees)
        {
            float newTime = Mathf.Max(minCoffeeTime, coffee.brewTime * (1f - cookSpeedReducePerLevel));
            if (newTime > minCoffeeTime) reachedLimit = false;
            coffee.brewTime = newTime;
        }

        cookSpeedMaxReached = reachedLimit;
    }

    private void ApplyRandomClientUpgrade()
    {
        if (ClientDatabase.Instance.clientTypes.Count == 0) return;

        int index = Random.Range(0, ClientDatabase.Instance.clientTypes.Count);
        var randomClient = ClientDatabase.Instance.clientTypes[index];

        lastClientName = randomClient.clientName;
        lastClientOldMult = randomClient.paymentMultiplier;
        randomClient.paymentMultiplier += randomClientBonus;
        lastClientNewMult = randomClient.paymentMultiplier;
    }

    private void ApplyRandomFoodPriceUpgrade()
    {
        if (MenuDatabase.Instance.foodItems.Count == 0) return;

        int index = Random.Range(0, MenuDatabase.Instance.foodItems.Count);
        var randomFood = MenuDatabase.Instance.foodItems[index];

        lastUpgradedFoodName = randomFood.foodName;
        lastFoodOldPrice = randomFood.price;

        float bonus = randomFood.price * (randomFoodBonusPercent / 100f);
        randomFood.price = Mathf.RoundToInt(randomFood.price + bonus);
        lastUpgradedFoodPrice = randomFood.price;
    }

    // ------------------ UI ------------------
    public void BuyUpgrade(int type)
    {
        switch (type)
        {
            case 0: UpgradePatience(); break;
            case 1: UpgradeCookSpeed(); break;
            case 2: UpgradeRandomClient(); break;
            case 3: UpgradeRandomFoodPrice(); break;
        }
    }

    // ------------------ СБРОС ------------------
    [ContextMenu("Reset All Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        money = 0;
        patienceLevel = 0;
        cookSpeedLevel = 0;
        randomClientMultLevel = 0;
        randomFoodPriceLevel = 0;
        lastUpgradedFoodName = "";
        lastUpgradedFoodPrice = 0;
        lastClientName = "";
        cookSpeedMaxReached = false;

        if (ClientDatabase.Instance != null)
            foreach (var client in ClientDatabase.Instance.clientTypes)
                client.ResetToDefault();

        if (MenuDatabase.Instance != null)
            foreach (var food in MenuDatabase.Instance.foodItems)
                food.ResetToDefault();

        Wallet.Instance?.UpdateUIImmediate(money);
    }

    // ------------------ Сохранение ------------------
    private void SaveProgress()
    {
        PlayerPrefs.SetInt(MoneyKey, money);
        PlayerPrefs.SetInt("PatienceLevel", patienceLevel);
        PlayerPrefs.SetInt("CookSpeedLevel", cookSpeedLevel);
        PlayerPrefs.SetInt("RandomClientLevel", randomClientMultLevel);
        PlayerPrefs.SetInt("RandomFoodLevel", randomFoodPriceLevel);
        PlayerPrefs.SetInt("CookSpeedMaxReached", cookSpeedMaxReached ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        money = PlayerPrefs.GetInt(MoneyKey, 0);
        patienceLevel = PlayerPrefs.GetInt("PatienceLevel", 0);
        cookSpeedLevel = PlayerPrefs.GetInt("CookSpeedLevel", 0);
        randomClientMultLevel = PlayerPrefs.GetInt("RandomClientLevel", 0);
        randomFoodPriceLevel = PlayerPrefs.GetInt("RandomFoodLevel", 0);
        cookSpeedMaxReached = PlayerPrefs.GetInt("CookSpeedMaxReached", 0) == 1;
        Wallet.Instance?.UpdateUIImmediate(money);
    }
}
