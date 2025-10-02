﻿using UnityEngine;
using UnityEngine.EventSystems;

public class FoodItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string foodName;

    [HideInInspector] public bool isFromCoffeeMachine = false;
    [HideInInspector] public CoffeeMachineUI coffeeMachine;

    private Vector3 startPosition;
    private Transform startParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        startParent = transform.parent;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
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

        TrayUI tray = eventData.pointerEnter ? eventData.pointerEnter.GetComponentInParent<TrayUI>() : null;

        if (tray != null && tray.AddFood(this))
        {
            Debug.Log($"Еда {foodName} добавлена на поднос!");

            if (isFromCoffeeMachine && coffeeMachine != null)
                coffeeMachine.TakeCoffee(); // только тут считается забранным
        }
        else
        {
            if (isFromCoffeeMachine && coffeeMachine != null)
            {
                // вернем кофе на автомат
                coffeeMachine.ReturnCoffee(this);
            }
            else
            {
                // обычная еда → вернуть на место
                transform.position = startPosition;
                transform.SetParent(startParent);
            }
        }
    }
}
