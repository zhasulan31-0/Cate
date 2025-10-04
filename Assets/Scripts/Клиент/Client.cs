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
    public Image clientPortrait;
    public RectTransform orderIconsParent;
    public GameObject orderIconPrefab;

    [Header("Настройки пузыря")]
    public float bubbleInnerPaddingTop = 8f;
    public float bubbleInnerPaddingBottom = 8f;
    public float maxSpacing = 12f;
    public float minSpacing = -40f;

    [Header("Данные клиента")]
    public ClientData clientData;

    [Header("Перемещение")]
    public float moveSpeed = 200f; // скорость движения
    private RectTransform targetSlot;
    private Vector3 originalSpawn;
    private bool hasOrdered = false;

    private Dictionary<string, (FoodItemData item, int count)> currentOrderDict = new Dictionary<string, (FoodItemData, int)>();
    private VerticalLayoutGroup vLayout;
    private ClientSpawner spawner;

    private int queueIndex = 0;

    public void SetSpawner(ClientSpawner s) => spawner = s;

    private void Awake()
    {
        if (orderIconsParent == null)
            Debug.LogError("orderIconsParent not assigned on Client!");

        vLayout = orderIconsParent.GetComponent<VerticalLayoutGroup>();
        if (vLayout == null)
            Debug.LogError("orderIconsParent must have VerticalLayoutGroup!");
    }

    private void Start()
    {
        if (clientData == null)
            clientData = ClientDatabase.Instance.GetRandomClientType();

        if (clientPortrait != null && clientData.clientImage != null)
            clientPortrait.sprite = clientData.clientImage;

        ShowOrderUI(false);
        originalSpawn = transform.position;

        StartCoroutine(MoveToQueue());
    }

    private IEnumerator MoveToQueue()
    {
        // Ждем, пока найдем очередь
        while (true)
        {
            (targetSlot, queueIndex) = spawner.GetShortestQueue();
            if (targetSlot != null) break;
            yield return null;
        }

        // Добавляемся в очередь слота
        spawner.AddToQueue(targetSlot, this);

        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one;
        float progress = 0f;

        // Ждем своей очереди, пока перед нами кто-то стоит
        while (queueIndex > 0)
        {
            queueIndex = spawner.GetQueueIndex(targetSlot, this);
            yield return null;
        }

        // Движение к позиции на слоте
        Vector3 targetPos = spawner.GetQueuePosition(targetSlot, this);
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            progress += Time.deltaTime * 2f;
            transform.localScale = Vector3.Lerp(startScale, endScale, Mathf.Clamp01(progress));
            yield return null;
        }

        transform.position = targetPos;
        transform.localScale = Vector3.one;

        // Клиент достиг слота, делает заказ
        hasOrdered = true;
        ShowOrderUI(true);
        MakeOrder();
    }

    public void LeaveSlot()
    {
        if (targetSlot != null)
        {
            spawner.RemoveFromQueue(targetSlot, this);
        }
        ShowOrderUI(false);
        StartCoroutine(MoveBackAndDestroy());
    }

    private IEnumerator MoveBackAndDestroy()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * 0.4f;
        float progress = 0f;

        while (Vector3.Distance(transform.position, originalSpawn) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalSpawn, moveSpeed * Time.deltaTime);
            progress += Time.deltaTime * 2f;
            transform.localScale = Vector3.Lerp(startScale, endScale, Mathf.Clamp01(progress));
            yield return null;
        }
        Destroy(gameObject);
    }

    private void ShowOrderUI(bool show)
    {
        if (orderBubble != null)
            orderBubble.SetActive(show);
    }

    public void MakeOrder()
    {
        currentOrderDict.Clear();
        foreach (Transform ch in orderIconsParent) Destroy(ch.gameObject);

        int needed = Random.Range(clientData.minOrders, clientData.maxOrders + 1);
        List<string> availableNames = MenuDatabase.Instance.foodPrefabs.Select(f => f.data.foodName).ToList();

        for (int i = 0; i < needed; i++)
        {
            string chosenName = null;

            if (clientData.favoriteFoods != null && clientData.favoriteFoods.Count > 0 && Random.value < 0.5f)
                chosenName = clientData.favoriteFoods[Random.Range(0, clientData.favoriteFoods.Count)].foodName;

            if (chosenName == null)
                chosenName = availableNames[Random.Range(0, availableNames.Count)];

            FoodItemData item = MenuDatabase.Instance.GetItemByName(chosenName);
            if (item == null) continue;

            if (currentOrderDict.ContainsKey(chosenName))
            {
                var old = currentOrderDict[chosenName];
                currentOrderDict[chosenName] = (old.item, old.count + 1);
            }
            else
            {
                currentOrderDict[chosenName] = (item, 1);
            }
        }

        foreach (var kv in currentOrderDict)
        {
            GameObject go = Instantiate(orderIconPrefab, orderIconsParent);
            Image icon = go.GetComponentInChildren<Image>();
            if (icon != null)
                icon.sprite = kv.Value.item.foodIcon;

            TextMeshProUGUI countText = go.GetComponentInChildren<TextMeshProUGUI>();
            if (countText != null)
                countText.text = kv.Value.count > 1 ? $"x{kv.Value.count}" : "";
        }

        AdjustBubbleSpacing();
        Debug.Log($"{clientData.clientName} заказал: {string.Join(", ", currentOrderDict.Select(kv => kv.Key + (kv.Value.count > 1 ? " x" + kv.Value.count : "")))}");
    }

    private void AdjustBubbleSpacing()
    {
        float bubbleHeight = orderIconsParent.rect.height - (bubbleInnerPaddingTop + bubbleInnerPaddingBottom);
        int iconCount = orderIconsParent.childCount;
        if (iconCount <= 1) { vLayout.spacing = 0; return; }

        float totalIconsHeight = 0f;
        for (int i = 0; i < iconCount; i++)
        {
            var child = orderIconsParent.GetChild(i) as RectTransform;
            var layout = child.GetComponent<LayoutElement>();
            float h = layout != null && layout.preferredHeight > 0 ? layout.preferredHeight : child.rect.height;
            totalIconsHeight += h;
        }

        float free = bubbleHeight - totalIconsHeight;
        float spacing = Mathf.Clamp(free / (iconCount - 1), minSpacing, maxSpacing);
        vLayout.spacing = spacing;
    }

    public bool CheckOrder(TrayUI tray)
    {
        var trayNames = tray.GetFoodNames();
        var dictTray = trayNames.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        bool equal = currentOrderDict.Count == dictTray.Count &&
                     currentOrderDict.All(kv => dictTray.ContainsKey(kv.Key) && dictTray[kv.Key] == kv.Value.count);

        if (equal) Debug.Log($"{clientData.clientName} доволен! Заплатил.");
        else Debug.Log($"{clientData.clientName} недоволен! Уходит с неправильным заказом.");

        LeaveSlot();
        return equal;
    }
}
