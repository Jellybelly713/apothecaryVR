using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerInteract : NetworkBehaviour
{
    public Transform rayOrigin;

    // Max distance the player can interact
    public float interactDistance = 2.5f;

    private void Update()
    {
        // allows the local player (owner) to run interaction
        if (!IsOwner) return;
        
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
            TryInteract();
    }

    private void TryInteract()
    {
        // If the ray origin is not assigned, do nothing
        if (rayOrigin == null) return;

        // Create a ray that starts at rayOrigin and points forward
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance)) return;

        // Check if the object hit has an interactable component
        var interactable = hit.collider.GetComponentInParent<Interactable>();

        interactable?.Interact();
    }
}