using UnityEngine;

// Ensures this object always has a Rigidbody attached
[RequireComponent(typeof(Rigidbody))]
public class PickupItem : MonoBehaviour
{
    private Rigidbody rb;

    private Collider[] allColliders;

    public bool IsHeld { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        allColliders = GetComponentsInChildren<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        IsHeld = true;

        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Disable all colliders to prevent collisions while holding the item
        foreach (var col in allColliders)
            col.enabled = false;

        // Attach to player's hold point
        transform.SetParent(holdPoint);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        IsHeld = false;

        // Detach from the player
        transform.SetParent(null);

        // Re-enable physics
        rb.isKinematic = false;
        rb.useGravity = true;

        foreach (var col in allColliders)
            col.enabled = true;
    }
}