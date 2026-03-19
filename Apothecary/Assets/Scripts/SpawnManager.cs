using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnHeightOffset = 0.75f;

    private readonly List<ulong> joinOrder = new List<ulong>();

    private void Awake()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += Approval;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.ConnectionApprovalCallback -= Approval;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void Approval(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        res.Approved = true;
        res.CreatePlayerObject = false;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawnPoints assigned on SpawnManager.");
            return;
        }

        if (!joinOrder.Contains(clientId))
            joinOrder.Add(clientId);

        int slot = Mathf.Clamp(joinOrder.IndexOf(clientId), 0, spawnPoints.Length - 1);

        var prefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;
        if (prefab == null)
        {
            Debug.LogError("PlayerPrefab not set on NetworkManager.");
            return;
        }

        Transform sp = spawnPoints[slot];

        Vector3 spawnPos = sp.position + Vector3.up * spawnHeightOffset;
        Quaternion spawnRot = Quaternion.Euler(0f, sp.eulerAngles.y, 0f);

        var playerObj = Instantiate(prefab, spawnPos, spawnRot);

        var cc = playerObj.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerObj.transform.SetPositionAndRotation(spawnPos, spawnRot);

        var movement = playerObj.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.ResetMovementState();

        Physics.SyncTransforms();

        if (cc != null) cc.enabled = true;

        var netObj = playerObj.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("Spawned player prefab is missing NetworkObject.");
            Destroy(playerObj);
            return;
        }

        netObj.SpawnAsPlayerObject(clientId, true);

        var visual = playerObj.GetComponent<PlayerCharacterVisual>();
        if (visual != null)
        {
            int id = (clientId == NetworkManager.ServerClientId) ? 0 : 1;
            visual.SetCharacterServer(id);
        }

        Debug.Log($"Spawned client {clientId} at slot {slot} -> {spawnPos}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        joinOrder.Remove(clientId);
    }
}