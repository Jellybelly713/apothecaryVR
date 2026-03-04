using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public Transform rayOrigin;
    public float interactDistance = 2.5f;

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
            TryInteract();
    }

    private void TryInteract()
    {
        if (rayOrigin == null) return;

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance)) return;

        var interactable = hit.collider.GetComponentInParent<Interactable>();
        interactable?.Interact();
    }
}