using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    //first player gets spawn 0, the second gets spawn 1.
    private readonly List<ulong> joinOrder = new List<ulong>();

    private void Awake()
    {
        // Register callbacks with the NetworkManager when script starts

        // Called when a new client requests to connect
        NetworkManager.Singleton.ConnectionApprovalCallback += Approval;

        // Called after a client connects
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        // Called when a client disconnects
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        // Remove callbacks when the object is destroyed

        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.ConnectionApprovalCallback -= Approval;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    // connection approval for incoming clients
    private void Approval(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        // Allow all connections
        res.Approved = true;

        // disable automatic player spawning to manually spawn players at specific spawn points
        res.CreatePlayerObject = false;
    }

    // Called whenever a client connects
    private void OnClientConnected(ulong clientId)
    {
        // Only the server should control spawning logic
        if (!NetworkManager.Singleton.IsServer) return;

        // Make sure spawn points exist
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawnPoints assigned on SpawnManager.");
            return;
        }

        if (!joinOrder.Contains(clientId))
            joinOrder.Add(clientId);

        // First player = slot 0, second player = slot 1, etc.
        int slot = Mathf.Clamp(joinOrder.IndexOf(clientId), 0, spawnPoints.Length - 1);

        // Get the player prefab from the NetworkManager
        var prefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;

        if (prefab == null)
        {
            Debug.LogError("PlayerPrefab not set on NetworkManager.");
            return;
        }

        Transform sp = spawnPoints[slot];

        // Instantiate the player object at spawn position
        var playerObj = Instantiate(prefab, sp.position, sp.rotation);

        // Get the NetworkObject component
        var netObj = playerObj.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId, true);

        // which character model this player should use
        var visual = playerObj.GetComponent<PlayerCharacterVisual>();

        if (visual != null)
        {
            // Host = Mage_01, client =Mage_02
            int id = (clientId == NetworkManager.ServerClientId) ? 0 : 1;

            visual.SetCharacterServer(id);
        }
    }

    // Called when a client disconnects from the server
    private void OnClientDisconnected(ulong clientId)
    {
        // Remove the player from the join order list
        joinOrder.Remove(clientId);
    }
}