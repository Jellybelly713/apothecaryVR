using UnityEngine;

public class NpcQueueMember : MonoBehaviour
{
    private NpcQueueManager manager;

    private Vector3 targetPos;
    private Quaternion targetRot;

    private float speed;

    private bool moving;

    // queue manager reference
    public void SetManager(NpcQueueManager m) => manager = m;

    public void TeleportTo(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);
        moving = false;
    }

    // Sets a destination and starts gliding toward it
    public void MoveTo(Vector3 pos, Quaternion rot, float glideSpeed)
    {
        targetPos = pos;
        targetRot = rot;
        speed = glideSpeed;
        moving = true;
    }

    void Update()
    {
        // Do nothing if this NPC is not moving
        if (!moving) return;

        // Move toward the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) <= 0.01f)
            moving = false;
    }

    // Checks if the NPC is close enough to a position
    public bool IsAt(Vector3 pos, float dist) => Vector3.Distance(transform.position, pos) <= dist;

    public void OnServed()
    {
        // destroy the NPC after they are served
        Destroy(gameObject);
    }
}