using System.Collections.Generic;
using UnityEngine;

public class ClientSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public RectTransform spawnPoint;
    public List<RectTransform> clientSlots;
    public GameObject clientPrefab;

    [Header("Очередь")]
    public float moveSpeed = 1f;

    private List<Client> activeClients = new List<Client>();
    private Dictionary<RectTransform, Client> slotOwners = new Dictionary<RectTransform, Client>();

    private void Awake()
    {
        foreach (var slot in clientSlots)
            slotOwners[slot] = null; // изначально все слоты пустые
    }

    public void SpawnClient()
    {
        GameObject go = Instantiate(clientPrefab, spawnPoint);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one * 0.4f;
        Client client = go.GetComponent<Client>();
        if (client != null)
        {
            client.SetSpawner(this);
            ClientData data = ClientDatabase.Instance.GetRandomClientType();
            client.Initialize(data);
            activeClients.Add(client);
        }
    }

    /// <summary>
    /// Возвращает свободный слот или null, если все заняты.
    /// </summary>
    public RectTransform GetFreeSlot(Client client)
    {
        foreach (var kv in slotOwners)
        {
            if (kv.Value == null) // слот свободен
            {
                slotOwners[kv.Key] = client; // занимаем
                return kv.Key;
            }
        }
        return null; // нет свободных
    }

    public void FreeSlot(RectTransform slot, Client client)
    {
        if (slot != null && slotOwners.ContainsKey(slot) && slotOwners[slot] == client)
        {
            slotOwners[slot] = null; // освобождаем слот
        }
    }

    public Vector3 GetSlotPosition(RectTransform slot)
    {
        return slot.position;
    }
}
