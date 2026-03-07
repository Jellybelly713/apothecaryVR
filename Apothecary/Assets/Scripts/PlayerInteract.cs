using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerInteract : NetworkBehaviour
{
    public Transform rayOrigin;

    public float interactDistance = 3f;

    public Transform holdPoint;

    private PickupItem heldItem;

    private void Update()
    {
        if (!IsOwner) return;

        if (Keyboard.current == null) return;

        // E to interact
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // If holding an item, drop it
            if (heldItem != null)
            {
                DropHeldItem();
            }
            else
            {
                TryPickupOrInteract();
            }
        }
    }

    private void TryPickupOrInteract()
    {
        if (rayOrigin == null) return;

        // Ray pointing forward from the player
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance)) return;

        // check if the object is something we can pick up
        PickupItem pickup = hit.collider.GetComponentInParent<PickupItem>();
        if (pickup != null)
        {
            PickUpItem(pickup);
            return;
        }

        var interactable = hit.collider.GetComponentInParent<Interactable>();

        interactable?.Interact();
    }

    private void PickUpItem(PickupItem item)
    {
        if (item == null) return;
        if (heldItem != null) return;
        if (holdPoint == null) return;

        heldItem = item;

        // Tells item to attach itself to the player
        heldItem.PickUp(holdPoint);
    }

    private void DropHeldItem()
    {
        if (heldItem == null) return;

        // drop and re-enable physics
        heldItem.Drop();

        heldItem = null;
    }
}