using UnityEngine;
using UnityEngine.EventSystems;

public class FoodItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Данные о еде (ScriptableObject)")]
    public FoodItemData data;

    [HideInInspector] public bool isFromCoffeeMachine = false;
    [HideInInspector] public CoffeeMachineUI coffeeMachine;
    [HideInInspector] public FoodShelf shelf;

    private Vector3 startPosition;
    private Transform startParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    // --- Новый компонент ---
    private AudioSource parentAudioSource;
    public event System.Action<FoodItem> onPlacedOnTray;

    public void OnPlacedOnTray()
    {
        onPlacedOnTray?.Invoke(this);
    }

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Проверяем, есть ли AudioSource у родителя (Canvas или Root)
        Transform root = canvas.transform;
        parentAudioSource = root.GetComponent<AudioSource>();
        if (parentAudioSource == null)
        {
            parentAudioSource = root.gameObject.AddComponent<AudioSource>();
            parentAudioSource.playOnAwake = false;
        }
    }

    public string GetName() => data != null ? data.foodName : "???";
    public Sprite GetIcon() => data != null ? data.foodIcon : null;

    // ========== DRAG ==========
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        startParent = transform.parent;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;

        // --- Проигрываем звук еды ---
        if (data != null && data.eatSound != null && parentAudioSource != null)
        {
            parentAudioSource.clip = data.eatSound;
            parentAudioSource.loop = false; // чтобы звук не зацикливался
            parentAudioSource.Play();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            transform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        bool placed = false;

        // проверка подносов
        foreach (var tray in FindObjectsOfType<TrayUI>())
        {
            RectTransform trayRect = tray.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(trayRect, eventData.position, canvas.worldCamera))
            {
                if (tray.AddFood(this))
                {
                    placed = true;

                    if (isFromCoffeeMachine && coffeeMachine != null)
                    {
                        coffeeMachine.TakeCoffee();
                        isFromCoffeeMachine = false;
                        coffeeMachine = null;
                    }
                }
            }
        }

        // если никуда не положили — вернуть на место
        if (!placed)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
        }

        // ✅ Если предмет был с полки и **ушёл с полки** → спавним новую еду
        if (shelf != null && transform.parent != shelf.transform)
        {
            shelf.ReplaceFood(this);
            shelf = null;
        }
    }

}
