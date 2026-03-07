using Unity.Netcode;
using UnityEngine;

public class LocalPlayerVisuals : NetworkBehaviour
{
    [SerializeField] private GameObject visualWorld;
    [SerializeField] private GameObject visualSelf;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // The owning player shouldnt their full world model
            if (visualWorld != null) visualWorld.SetActive(false);

            if (visualSelf != null) visualSelf.SetActive(true);
        }
        else
        {
            // Other players see full character model
            if (visualWorld != null) visualWorld.SetActive(true);

            if (visualSelf != null) visualSelf.SetActive(false);
        }
    }
}