using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    // Reference to the queue manager that handles NPC positions
    [SerializeField] private NpcQueueManager queue;

    [SerializeField] private GameObject[] npcPrefabs;

    // Time between each NPC spawn
    [SerializeField] private float spawnDelay = 6f;

    private float timer;

    void Update()
    {
        if (queue == null) return;

        // Stop if there are no NPC prefabs to spawn
        if (npcPrefabs == null || npcPrefabs.Length == 0) return;

        timer += Time.deltaTime;

        // if enough time has passed, spawn a new NPC
        if (timer >= spawnDelay)
        {
            timer = 0f;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        if (queue.IsFull) return;

        // Picks a random NPC prefab from the list
        GameObject prefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        // Spawn the NPC at the spawners position
        GameObject npc = Instantiate(prefab, transform.position, Quaternion.identity);

        var member = npc.GetComponent<NpcQueueMember>();
        if (member == null)
        {
            Debug.LogWarning("NPC missing NpcQueueMember component.");
            Destroy(npc);
            return;
        }

        if (!queue.TryEnqueue(member))
        {
            Destroy(npc);
        }
    }
}