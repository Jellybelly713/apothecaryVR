using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickStart : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null) return;

        // Press 'H' to start the game as Host
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            // Makes sure a NetworkManager exists
            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("No NetworkManager.Singleton");
                return;
            }

            Debug.Log("Starting Host");
            bool ok = NetworkManager.Singleton.StartHost();

            // Log whether the host started
            Debug.Log("StartHost returned: " + ok);
        }

        // Press 'C' to start the game as Client
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("No NetworkManager.Singleton");
                return;
            }

            Debug.Log("Starting Client");
            bool ok = NetworkManager.Singleton.StartClient();

            // Log whether the client started
            Debug.Log("StartClient returned: " + ok);
        }
    }
}