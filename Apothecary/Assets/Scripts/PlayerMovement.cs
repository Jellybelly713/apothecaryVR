using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 4.5f;

    // Gravity to player each frame
    public float gravity = -20f;

    public float jumpHeight = 1.2f;

    // Time window allowing the player to still jump after leaving the ground
    public float jumpWindow = 0.12f;

    // Time window allowing a jump input slightly before landing to make jumping feel responsive
    public float jumpBuffer = 0.12f;

    private CharacterController cc;

    // velocity of the player
    private Vector3 velocity;

    // how long the player has been off the ground
    private float timeSinceGrounded;

    // how long since the jump key was pressed
    private float timeSinceJumpPressed;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!IsOwner) return;

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

        // Normalize input so diagonal movement is not faster
        input = input.normalized;

        // Convert input direction into world space movement
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        cc.Move(move * moveSpeed * Time.deltaTime);

        // JUMP LOGIC
        // how long the player has been grounded
        timeSinceGrounded = cc.isGrounded ? 0f : timeSinceGrounded + Time.deltaTime;
 
        timeSinceJumpPressed += Time.deltaTime;

        bool canJump = timeSinceGrounded <= jumpWindow;
        bool wantsJump = timeSinceJumpPressed <= jumpBuffer;

        // Perform jump if both conditions are true
        if (canJump && wantsJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            timeSinceJumpPressed = 999f;
        }

        // GRAVITY
        if (cc.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // Apply gravity over time
        velocity.y += gravity * Time.deltaTime;

        // Move the player vertically
        cc.Move(velocity * Time.deltaTime);
    }
    public void ResetMovementState()
    {
        // after spawning, remove vertical velicity
        velocity = Vector3.zero;
    }
}