using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodShelf : MonoBehaviour
{
    [Header("Настройки полки")]
    public FoodItemData foodData;      // какой предмет генерируется (например, кекс)
    public GameObject foodPrefab;      // префаб с FoodItem
    public int startCount = 2;         // сколько сразу показывать
    public float respawnDelay = 0.5f;  // задержка перед появлением нового
    public float animTime = 0.3f;      // длительность анимации

    private List<FoodItem> itemsOnShelf = new List<FoodItem>();
    private HorizontalLayoutGroup layoutGroup;
    private bool isRespawning = false;

    private void Awake()
    {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            Debug.LogError("FoodShelf требует VerticalLayoutGroup на объекте!");
        }
    }

    private void Start()
    {
        // генерируем стартовую стопку
        for (int i = 0; i < startCount; i++)
        {
            SpawnFood();
        }
    }

    private void SpawnFood()
    {
        GameObject go = Instantiate(foodPrefab, transform);
        go.transform.localScale = Vector3.zero;

        FoodItem item = go.GetComponent<FoodItem>();
        item.data = foodData;
        item.shelf = this; // 🔹 назначаем полку

        itemsOnShelf.Add(item);
        StartCoroutine(ScaleIn(go.transform));
    }

    private IEnumerator ScaleIn(Transform target)
    {
        float t = 0f;
        while (t < animTime)
        {
            t += Time.deltaTime;
            float k = t / animTime;
            target.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, k);
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    /// <summary>
    /// Когда игрок взял еду → обновляем стопку
    /// </summary>
    public void ReplaceFood(FoodItem takenItem)
    {
        if (itemsOnShelf.Contains(takenItem))
            itemsOnShelf.Remove(takenItem);

        // плавный релаут
        StartCoroutine(SmoothRebuildLayout());

        // задержка перед спавном нового
        if (!isRespawning)
            StartCoroutine(RespawnFoodAfterDelay());
    }

    private IEnumerator RespawnFoodAfterDelay()
    {
        isRespawning = true;
        yield return new WaitForSeconds(respawnDelay);
        SpawnFood();
        isRespawning = false;
    }

    private IEnumerator SmoothRebuildLayout()
    {
        // ждём 1 кадр чтобы LayoutGroup пересобрался
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        yield return null;

        // анимируем все элементы (их позиции/scale)
        float t = 0f;
        Vector3[] startPos = new Vector3[itemsOnShelf.Count];
        Vector3[] targetPos = new Vector3[itemsOnShelf.Count];

        for (int i = 0; i < itemsOnShelf.Count; i++)
        {
            startPos[i] = itemsOnShelf[i].transform.localPosition;
            targetPos[i] = itemsOnShelf[i].transform.localPosition; // Layout уже перестроил
        }

        while (t < animTime)
        {
            t += Time.deltaTime;
            float k = t / animTime;

            for (int i = 0; i < itemsOnShelf.Count; i++)
            {
                if (itemsOnShelf[i] != null)
                    itemsOnShelf[i].transform.localPosition = Vector3.Lerp(startPos[i], targetPos[i], k);
            }

            yield return null;
        }
    }
}
