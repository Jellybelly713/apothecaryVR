using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerLook : NetworkBehaviour
{
    // allows vertical rotation without tilting the player
    public Transform cameraPivot;

    // Mouse sens multiplier
    public float mouseSensitivity = 0.08f;

    // Max vertical look angle (prevents flipping the camera upside down)
    public float pitchClamp = 80f;

    // Current vertical rotation angle
    private float pitch;

    private void OnEnable()
    {
        // Lock the mouse cursor to the center of screen
        Cursor.lockState = CursorLockMode.Locked;

        // Hide the cursor while playing
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        if (Mouse.current == null) return;

        // mouse movement delta
        Vector2 delta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        float mouseX = delta.x;
        float mouseY = delta.y;

        // Horizontal rotation
        // Rotates the player left/right
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);

        // keeps the player's body upright
        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Press 'Esc' to unlock and show the mouse cursor
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}