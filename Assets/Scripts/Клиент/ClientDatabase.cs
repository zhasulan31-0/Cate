using UnityEngine;
using System.Collections.Generic;

public class ClientDatabase : MonoBehaviour
{
    public static ClientDatabase Instance;

    [Header("Список всех типов клиентов")]
    public List<ClientData> clientTypes = new List<ClientData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public ClientData GetRandomClientType()
    {
        if (clientTypes.Count == 0) return null;
        int index = Random.Range(0, clientTypes.Count);
        return clientTypes[index];
    }
}
