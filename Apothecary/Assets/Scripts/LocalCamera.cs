using Unity.Netcode;
using UnityEngine;

public class LocalCamera : NetworkBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private AudioListener listener;

    public override void OnNetworkSpawn()
    {
        bool local = IsOwner;
        if (cam) cam.enabled = local;
        if (listener) listener.enabled = local;
    }
}
