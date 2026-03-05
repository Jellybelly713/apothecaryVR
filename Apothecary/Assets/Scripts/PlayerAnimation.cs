using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator worldAnimator;

    public float smooth = 10f;

    private Vector3 lastPos;
    private float speed01;

    private void Start()
    {
        lastPos = transform.position;
    }

    private void Update()
    {
        // speed from position change
        Vector3 delta = (transform.position - lastPos);
        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPos = transform.position;

        // map to 0-1-ish for Animator
        float target = Mathf.Clamp01(speed / 3.5f);
        speed01 = Mathf.Lerp(speed01, target, 1f - Mathf.Exp(-smooth * Time.deltaTime));

        if (worldAnimator != null)
            worldAnimator.SetFloat("Speed", speed01);
    }
}
