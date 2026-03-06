using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcQueueManager : MonoBehaviour
{
    // Queue positions in order: Slot_0 = front of the line at the window
    [SerializeField] private Transform[] slots;

    [SerializeField] private float glideSpeed = 4.5f;
    [SerializeField] private float arriveDistance = 0.03f;

    // Stores NPCs
    private readonly List<NpcQueueMember> members = new List<NpcQueueMember>();

    // Queue direction
    private bool forward = true;

    // Prevents multiple shifts from happening at the same time
    private bool isShifting;

    // Total number slots
    public int Capacity => slots != null ? slots.Length : 0;

    // True if the line is full
    public bool IsFull => slots != null && members.Count >= slots.Length;

    public bool TryEnqueue(NpcQueueMember npc)
    {
        // Fails if there are no queue slots or if the queue is full
        if (slots == null || slots.Length == 0) return false;

        if (members.Count >= slots.Length) return false;

        // Add the new NPC to the queue list
        members.Add(npc);

        // NPC reference
        npc.SetManager(this);

        // Find which slot this NPC should move to
        int slotIndex = GetSlotIndexForMember(members.Count - 1);

        // Tell the NPC to go to next slot
        npc.MoveTo(slots[slotIndex].position, slots[slotIndex].rotation, glideSpeed);

        return true;
    }

    public void ServeFrontNpc()
    {
        if (members.Count == 0) return;

        if (isShifting) return;

        var served = members[0];

        members.RemoveAt(0);

        served.OnServed();

        StartCoroutine(ShiftLine());
    }

    private IEnumerator ShiftLine()
    {
        isShifting = true;

        // Move each NPC into its new slot
        for (int i = 0; i < members.Count; i++)
        {
            int slotIndex = GetSlotIndexForMember(i);
            members[i].MoveTo(slots[slotIndex].position, slots[slotIndex].rotation, glideSpeed);
        }

        // Wait until every NPC has reached its new position
        while (true)
        {
            bool allArrived = true;

            for (int i = 0; i < members.Count; i++)
            {
                int slotIndex = GetSlotIndexForMember(i);

                if (!members[i].IsAt(slots[slotIndex].position, arriveDistance))
                {
                    allArrived = false;
                    break;
                }
            }

            if (allArrived) break;
            yield return null;
        }

        isShifting = false;
    }

    private int GetSlotIndexForMember(int memberIndex)
    {
        // Safety fallback if slots are missing
        if (slots == null || slots.Length == 0) return 0;

        if (forward)
        {
            // first NPC goes to Slot_0, second to Slot_1...
            return Mathf.Clamp(memberIndex, 0, slots.Length - 1);
        }
        else
        {
            // first NPC goes to last slot, second to second-last...
            int last = slots.Length - 1;
            return Mathf.Clamp(last - memberIndex, 0, last);
        }
    }

    private void Update()
    {
        // Testing: 'K' to serve the NPC and shift the line
        if (Input.GetKeyDown(KeyCode.K))
        {
            ServeFrontNpc();
        }
    }
}