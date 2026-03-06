using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Reference to the Animator that controls the character's animations
    public Animator worldAnimator;

    // Controls animation speed transitions
    public float smooth = 10f;

    // Stores player's position from the previous frame
    private Vector3 lastPos;

    // Smoothed animation speed value between 0 and 1
    private float speed01;

    private void Start()
    {
        // calculate movement on the first update
        lastPos = transform.position;
    }

    private void Update()
    {
        // how far the player moved since the last frame
        Vector3 delta = (transform.position - lastPos);

        // Convert the movement distance into a speed value
        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);

        // Update the stored position for the next frame
        lastPos = transform.position;

        // Normalize the speed into a 0–1 range for animation blending
        float target = Mathf.Clamp01(speed / 3.5f);

        // prevents animation jitter when the player stops or changes direction
        speed01 = Mathf.Lerp(speed01, target, 1f - Mathf.Exp(-smooth * Time.deltaTime));

        // blend between idle/walk/run
        if (worldAnimator != null && worldAnimator.isActiveAndEnabled)
            worldAnimator.SetFloat("Speed", speed01);
    }
}