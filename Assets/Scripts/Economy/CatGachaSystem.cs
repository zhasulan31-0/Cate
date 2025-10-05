using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CatItem
{
    public Sprite catImage;
    public string rarity;
}

public class CatGachaSystem : MonoBehaviour
{
    [Header("UI")]
    public GameObject gachaPanel;
    public Button openPanelButton;
    public Button closePanelButton;
    public Button rollButton;
    public Image displayImage;
    public TMP_Text resultText;
    public TMP_Text rarityText;

    [Header("Настройки кейса")]
    public int caseCost = 20;
    public float rollDuration = 2f;
    public float rollSpeed = 0.08f;

    private List<CatItem> catList = new List<CatItem>();
    private bool isRolling = false;

    void Start()
    {
        LoadCatsFromResources();

        if (openPanelButton != null)
            openPanelButton.onClick.AddListener(OpenPanel);

        if (closePanelButton != null)
            closePanelButton.onClick.AddListener(ClosePanel);

        if (rollButton != null)
            rollButton.onClick.AddListener(StartRoll);

        if (gachaPanel != null)
            gachaPanel.SetActive(false);
    }

    void LoadCatsFromResources()
    {
        Sprite[] catSprites = Resources.LoadAll<Sprite>("Cats");
        catList.Clear();

        if (catSprites.Length == 0)
        {
            Debug.LogWarning("❌ Нет картинок в папке Resources/Cats!");
            return;
        }

        for (int i = 0; i < catSprites.Length; i++)
        {
            string rarity = "Common";
            if (i >= 6 && i < 12) rarity = "Rare";
            else if (i >= 12) rarity = "Super Rare";

            catList.Add(new CatItem
            {
                catImage = catSprites[i],
                rarity = rarity
            });
        }

        Debug.Log($"✅ Загружено котов: {catList.Count}");
    }

    void OpenPanel()
    {
        if (gachaPanel != null)
            gachaPanel.SetActive(true);
    }

    void ClosePanel()
    {
        if (gachaPanel != null)
            gachaPanel.SetActive(false);
    }

    void StartRoll()
    {
        if (isRolling || Wallet.Instance == null || Wallet.Instance.GetMoney() < caseCost)
            return;

        Wallet.Instance.SpendMoney(caseCost);
        StartCoroutine(RollAnimation());
    }

    IEnumerator RollAnimation()
    {
        if (catList.Count == 0)
            yield break;

        isRolling = true;
        float timer = 0f;
        int currentIndex = 0;

        while (timer < rollDuration)
        {
            currentIndex = Random.Range(0, catList.Count);
            var cat = catList[currentIndex];

            displayImage.sprite = cat.catImage;
            resultText.text = cat.catImage != null ? cat.catImage.name : "???";
            rarityText.text = cat.rarity;

            timer += rollSpeed;
            yield return new WaitForSeconds(rollSpeed);
        }

        CatItem finalCat = catList[currentIndex];
        displayImage.sprite = finalCat.catImage;
        resultText.text = finalCat.catImage != null ? finalCat.catImage.name : "???";
        rarityText.text = finalCat.rarity;

        isRolling = false;
    }
}
