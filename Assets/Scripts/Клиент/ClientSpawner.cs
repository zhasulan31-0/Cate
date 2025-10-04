using System.Collections.Generic;
using UnityEngine;

public class ClientSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public RectTransform spawnPoint;
    public List<RectTransform> clientSlots;
    public GameObject clientPrefab;

    [Header("Очередь")]
    public float queueSpacing = 50f; // вертикальное смещение клиентов

    private List<Client> activeClients = new List<Client>();
    private Dictionary<RectTransform, List<Client>> slotQueues = new Dictionary<RectTransform, List<Client>>();

    private void Awake()
    {
        foreach (var slot in clientSlots)
            slotQueues[slot] = new List<Client>();
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
            activeClients.Add(client);
        }
    }

    public (RectTransform slot, int index) GetShortestQueue()
    {
        RectTransform chosenSlot = null;
        int minCount = int.MaxValue;

        foreach (var kv in slotQueues)
        {
            if (kv.Value.Count < minCount)
            {
                minCount = kv.Value.Count;
                chosenSlot = kv.Key;
            }
        }

        if (chosenSlot == null) return (null, 0);
        return (chosenSlot, slotQueues[chosenSlot].Count);
    }

    public void AddToQueue(RectTransform slot, Client client)
    {
        slotQueues[slot].Add(client);
    }

    public void RemoveFromQueue(RectTransform slot, Client client)
    {
        if (!slotQueues.ContainsKey(slot)) return;
        slotQueues[slot].Remove(client);
        // Vertical Layout Group обновит позиции автоматически
    }

    public int GetQueueIndex(RectTransform slot, Client client)
    {
        if (!slotQueues.ContainsKey(slot)) return 0;
        return slotQueues[slot].IndexOf(client);
    }

    public Vector3 GetQueuePosition(RectTransform slot, Client client)
    {
        int index = GetQueueIndex(slot, client);
        return slot.position + new Vector3(0, -index * queueSpacing, 0);
    }
}
