using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4.5f;
    public float gravity = -20f;
    public float jumpHeight = 1.2f;

    public float jumpWindow = 0.12f;
    public float jumpBuffer = 0.12f;

    private CharacterController cc;
    private Vector3 velocity;

    private float timeSinceGrounded;
    private float timeSinceJumpPressed;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // WASD
        Vector2 input = Vector2.zero;
        var kb = Keyboard.current;

        if (kb != null)
        {
            input.x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
            input.y = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);

            if (kb.spaceKey.wasPressedThisFrame)
                timeSinceJumpPressed = 0f;
        }

        input = input.normalized;
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        cc.Move(move * moveSpeed * Time.deltaTime);

        // Timers
        timeSinceGrounded = cc.isGrounded ? 0f : timeSinceGrounded + Time.deltaTime;
        timeSinceJumpPressed += Time.deltaTime;

        // Jump
        bool canJump = timeSinceGrounded <= jumpWindow;
        bool wantsJump = timeSinceJumpPressed <= jumpBuffer;

        if (canJump && wantsJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            timeSinceJumpPressed = 999f;
        }

        // --- Gravity ---
        if (cc.isGrounded && velocity.y < 0f) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}