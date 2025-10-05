using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        // Проверяем только поднос
        TrayUI tray = eventData.pointerDrag.GetComponent<TrayUI>();
        if (tray != null)
        {
            TrashTray(tray);
        }
    }

    public void TrashTray(TrayUI tray)
    {
        Debug.Log("Поднос выброшен в мусорку вместе с едой!");
        tray.DestroyTray();
    }
}
