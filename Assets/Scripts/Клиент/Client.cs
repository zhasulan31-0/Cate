using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Client : MonoBehaviour
{
    [Header("UI заказа")]
    public GameObject orderBubble;
    public RectTransform orderIconsParent;
    public GameObject orderIconPrefab;

    [Header("Настройки пузыря")]
    public float bubbleInnerPaddingTop = 8f;
    public float bubbleInnerPaddingBottom = 8f;
    public float maxSpacing = 20f;
    public float minSpacing = 10f;

    [Header("UI терпения")]
    public Slider patienceSlider;

    [Header("Данные клиента")]
    public ClientData clientData;

    [Header("Перемещение")]
    public float moveSpeed = 200f;
    private RectTransform targetSlot;
    private Vector3 originalSpawn;
    private bool hasOrdered = false;

    private Dictionary<string, (FoodItemData item, int count)> currentOrderDict = new Dictionary<string, (FoodItemData, int)>();
    private VerticalLayoutGroup vLayout;
    private ClientSpawner spawner;

    [Header("Images и анимации")]
    public Image portraitImage;     // UI Image портрета
    public Animator portraitAnimator; // Animator портрета с Walk/Idle
    public Image bodyImage;         // Image на Body для анимаций радости
    public Animator bodyAnimator;   // Animator на Body с Happy анимацией

    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private static readonly int IdleHash = Animator.StringToHash("Idle");

    [Header("Звук")]
    public AudioSource audioSource;

    private Coroutine patienceCoroutine;
    private bool isWaiting = false;

    public void SetSpawner(ClientSpawner s) => spawner = s;

    public void Initialize(ClientData data)
    {
        clientData = data;

        if (bodyImage != null) bodyImage.enabled = false;
        if (portraitImage != null)
        {
            portraitImage.enabled = true;
            SetPortrait(clientData.clientImage);
        }

        if (portraitAnimator != null) portraitAnimator.enabled = true;
    }

    private void Awake()
    {
        if (orderIconsParent == null)
            Debug.LogError("orderIconsParent not assigned on Client!");

        vLayout = orderIconsParent.GetComponent<VerticalLayoutGroup>();
        if (vLayout == null)
            Debug.LogError("orderIconsParent must have VerticalLayoutGroup!");
        if (patienceSlider != null)
            patienceSlider.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (ClientDatabase.Instance == null)
        {
            Debug.LogError("ClientDatabase.Instance == null!");
            return;
        }

        if (clientData == null)
        {
            clientData = ClientDatabase.Instance.GetRandomClientType();
            if (clientData == null)
            {
                Debug.LogError("clientData не назначен и ClientDatabase пустой!");
                return;
            }
            else
            {
                Debug.Log($"clientData назначен: {clientData.clientName}");
            }
        }

        if (orderBubble != null && !orderBubble.activeSelf)
            orderBubble.SetActive(true);

        if (clientData.clientImage != null)
            SetPortrait(clientData.clientImage);

        ShowOrderUI(false);
        originalSpawn = transform.position;
        transform.SetAsFirstSibling();

        StartCoroutine(MoveToQueue());
    }

    public void SetPortrait(Sprite newSprite)
    {
        if (portraitImage != null)
        {
            portraitImage.sprite = newSprite;
            portraitImage.SetNativeSize();
            portraitImage.canvasRenderer.SetColor(Color.white);
            portraitImage.enabled = true;
        }
    }

    private IEnumerator MoveToQueue()
    {
        transform.SetAsLastSibling();
        if (portraitAnimator != null) portraitAnimator.Play(WalkHash);

        // ждём свободного слота
        while (true)
        {
            targetSlot = spawner.GetFreeSlot(this);
            if (targetSlot != null) break;
            yield return null;
        }

        // идём к позиции
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one;
        Vector3 startPos = transform.position;
        Vector3 targetPos = spawner.GetSlotPosition(targetSlot);

        float distance = Vector3.Distance(startPos, targetPos);
        float travelled = 0f;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            travelled += step;
            float t = Mathf.Clamp01(travelled / distance);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.localScale = endScale;

        hasOrdered = true;
        ShowOrderUI(true);
        MakeOrder();

        if (portraitAnimator != null) portraitAnimator.Play(IdleHash);
    }

    public void LeaveSlot(bool correctOrder = false)
    {
        if (targetSlot != null)
        {
            spawner.FreeSlot(targetSlot, this);
            targetSlot = null;
        }

        ShowOrderUI(false);
        transform.SetAsFirstSibling();

        if (correctOrder && clientData != null)
        {
            PlayHappyAnimation();
        }
        else
        {
            if (portraitAnimator != null) portraitAnimator.Play(WalkHash);
        }

        // ✅ Уведомляем DayManager о результате клиента
        DayManager dayManager = FindObjectOfType<DayManager>();
        if (dayManager != null)
        {
            if (correctOrder)
            {
                // уже вызывается в CheckOrder при успешной подаче
            }
            else
            {
                // ❗ Ушёл недовольным, уведомляем
                dayManager.NotifyClientFinished(false, 0);
            }
        }

        StartCoroutine(MoveBackAndDestroy(correctOrder));
    }


    private void PlayHappyAnimation()
    {
        if (portraitImage != null) portraitImage.enabled = false;
        if (portraitAnimator != null) portraitAnimator.enabled = false;

        if (bodyImage != null) bodyImage.enabled = true;
        if (bodyAnimator != null && clientData != null)
            bodyAnimator.Play(clientData.happyAnimationName);

        if (audioSource != null && clientData.happySound != null)
            audioSource.PlayOneShot(clientData.happySound);
    }

    private IEnumerator MoveBackAndDestroy(bool happy)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * 0.4f;
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, originalSpawn);
        float travelled = 0f;

        while (Vector3.Distance(transform.position, originalSpawn) > 0.01f)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, originalSpawn, step);

            travelled += step;
            float t = Mathf.Clamp01(travelled / distance);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        transform.position = originalSpawn;
        transform.localScale = endScale;

        if (happy && bodyImage != null) bodyImage.enabled = false;
        if (happy && bodyAnimator != null) bodyAnimator.enabled = false;

        Destroy(gameObject);
    }

    private void ShowOrderUI(bool show)
    {
        if (orderBubble == null) return;

        orderBubble.SetActive(show);

        // Делаем пузырь выше по локальной иерархии относительно его родителя
        orderBubble.transform.SetAsLastSibling();
    }



    public void MakeOrder()
    {
        currentOrderDict.Clear();
        foreach (Transform ch in orderIconsParent) Destroy(ch.gameObject);

        int needed = Random.Range(Mathf.Max(1, clientData.minOrders), clientData.maxOrders + 1);
        List<string> availableNames = MenuDatabase.Instance.foodItems.Select(f => f.foodName).ToList();

        for (int i = 0; i < needed; i++)
        {
            string chosenName = null;
            if (clientData.favoriteFoods != null && clientData.favoriteFoods.Count > 0 && Random.value < 0.5f)
                chosenName = clientData.favoriteFoods[Random.Range(0, clientData.favoriteFoods.Count)].foodName;
            if (string.IsNullOrEmpty(chosenName))
                chosenName = availableNames[Random.Range(0, availableNames.Count)];
            FoodItemData item = MenuDatabase.Instance.GetItemByName(chosenName);
            if (item == null) continue;
            if (currentOrderDict.ContainsKey(chosenName))
                currentOrderDict[chosenName] = (currentOrderDict[chosenName].item, currentOrderDict[chosenName].count + 1);
            else
                currentOrderDict[chosenName] = (item, 1);
        }

        if (currentOrderDict.Count == 0)
        {
            var fallback = MenuDatabase.Instance.GetUniqueRandomItem();
            if (fallback != null)
                currentOrderDict[fallback.foodName] = (fallback, 1);
        }

        foreach (var kv in currentOrderDict)
        {
            GameObject go = Instantiate(orderIconPrefab, orderIconsParent);
            Image icon = go.GetComponentInChildren<Image>();
            if (icon != null) icon.sprite = kv.Value.item.foodIcon;

            TextMeshProUGUI countText = go.GetComponentInChildren<TextMeshProUGUI>();
            if (countText != null) countText.text = kv.Value.count > 1 ? $"x{kv.Value.count}" : "";
        }

        AdjustBubbleSpacing();
        Debug.Log($"{clientData.clientName} заказал: {string.Join(", ", currentOrderDict.Select(kv => kv.Key + (kv.Value.count > 1 ? " x" + kv.Value.count : "")))}");

        // ✅ Запускаем терпение после заказа
        StartPatienceTimer();
    }

    private void AdjustBubbleSpacing()
    {
        int iconCount = orderIconsParent.childCount;
        if (iconCount <= 1)
        {
            vLayout.spacing = 0;
            return;
        }

        float spacing = Mathf.Lerp(maxSpacing, minSpacing, (float)(iconCount - 1) / 5f);
        vLayout.spacing = spacing;
    }

    public bool CheckOrder(TrayUI tray)
    {
        if (!hasOrdered)
        {
            Debug.Log($"{clientData.clientName} ещё не сделал заказ!");
            return false;
        }

        var trayData = tray.GetFoodData();
        var dictTray = trayData.GroupBy(x => x.foodName).ToDictionary(g => g.Key, g => g.Count());

        bool equal = currentOrderDict.Count == dictTray.Count &&
                     currentOrderDict.All(kv => dictTray.ContainsKey(kv.Value.item.foodName) &&
                                                dictTray[kv.Value.item.foodName] == kv.Value.count);

        LeaveSlot(equal);

        if (equal)
        {
            if (equal)
            {
                int basePayment = tray.GetTotalPrice();
                float multiplier = clientData != null ? clientData.paymentMultiplier : 1f;
                int finalPayment = Mathf.RoundToInt(basePayment * multiplier);
                DayManager dayManager = FindObjectOfType<DayManager>();

                // Добавляем деньги игроку
                UpgradeManager.Instance?.AddMoney(finalPayment);

                // Подробный отчёт в консоль
                Debug.Log($"💰 Клиент {clientData.clientName} оплатил заказ:");
                Debug.Log($"   ├ Базовая сумма: {basePayment}$");
                Debug.Log($"   ├ Множитель клиента: x{multiplier:F2}");
                Debug.Log($"   └ Итоговая сумма: {finalPayment}$");
                dayManager.NotifyClientFinished(true, finalPayment);

                OnOrderServed(); // останавливаем таймер терпения
            }

        }

        return equal;
    }

    private void StartPatienceTimer()
    {
        if (patienceSlider != null)
        {
            patienceSlider.gameObject.SetActive(true); // активируется только после заказа
            patienceSlider.maxValue = clientData.patienceTime;
            patienceSlider.value = clientData.patienceTime;
        }

        if (patienceCoroutine != null)
            StopCoroutine(patienceCoroutine);
        patienceCoroutine = StartCoroutine(PatienceCountdown());
    }


    private IEnumerator PatienceCountdown()
    {
        isWaiting = true;
        float timer = clientData.patienceTime;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (patienceSlider != null)
                patienceSlider.value = timer;

            yield return null;
        }

        isWaiting = false;
        Debug.Log($"{clientData.clientName} потерял терпение и ушёл!");
        LeaveSlot(false);
        if (patienceSlider != null)
            patienceSlider.gameObject.SetActive(false);
    }

    public void OnOrderServed()
    {
        if (patienceCoroutine != null)
            StopCoroutine(patienceCoroutine);
        if (patienceSlider != null)
            patienceSlider.gameObject.SetActive(false);
        isWaiting = false;
    }
}
