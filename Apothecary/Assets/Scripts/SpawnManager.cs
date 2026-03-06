using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Spawn points
    [SerializeField] private Transform[] spawnPoints;

    // Tracks player join order so spawn positions stay consistent
    private readonly List<ulong> joinOrder = new List<ulong>();

    private void Awake()
    {
        // networking callbacks
        NetworkManager.Singleton.ConnectionApprovalCallback += Approval;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        // Unregister callbacks when this object is destroyed
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.ConnectionApprovalCallback -= Approval;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void Approval(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        // all players can connect
        res.Approved = true;

        // Disable automatic player spawning since its manual
        res.CreatePlayerObject = false;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only the server should spawn players
        if (!NetworkManager.Singleton.IsServer) return;

        // Make sure spawn points are assigned
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawnPoints assigned on SpawnManager.");
            return;
        }

        // Track join order
        if (!joinOrder.Contains(clientId))
            joinOrder.Add(clientId);

        // First player = slot 0, second = slot 1
        int slot = Mathf.Clamp(joinOrder.IndexOf(clientId), 0, spawnPoints.Length - 1);

        // Get the player prefab from NetworkManager
        var prefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;
        if (prefab == null)
        {
            Debug.LogError("PlayerPrefab not set on NetworkManager.");
            return;
        }

        Transform sp = spawnPoints[slot];

        // raise the spawn position so the player doesn't clip into the floor
        Vector3 spawnPos = sp.position + Vector3.up * 0.75f;
        Quaternion spawnRot = sp.rotation;

        // Create player object
        var playerObj = Instantiate(prefab, spawnPos, spawnRot);

        // Temporarily disable CharacterController so it doesnt mess with the spawn position
        var cc = playerObj.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Force spawn position and rotation
        playerObj.transform.SetPositionAndRotation(spawnPos, spawnRot);

        // Reset movement values after spawning
        var movement = playerObj.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ResetMovementState();
        }

        // enable CharacterController again
        if (cc != null) cc.enabled = true;

        // Spawn the networked player object
        var netObj = playerObj.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId, true);

        // Assign which character model
        var visual = playerObj.GetComponent<PlayerCharacterVisual>();
        if (visual != null)
        {
            // Host = Mage_01, client = Mage_02
            int id = (clientId == NetworkManager.ServerClientId) ? 0 : 1;
            visual.SetCharacterServer(id);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // Remove disconnected player from join order tracking
        joinOrder.Remove(clientId);
    }
}