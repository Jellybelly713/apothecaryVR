using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [SerializeField] private NpcQueueManager queue;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering zone is a deliverable
        IngredientItem item = other.GetComponent<IngredientItem>();
        if (item == null) return;

        // Get NPC at the front of the line
        NpcQueueMember frontNpc = queue != null ? queue.FrontNpc : null;
        if (frontNpc == null) return;

        // Get NPC's order
        CustomerOrder order = frontNpc.GetComponent<CustomerOrder>();
        if (order == null) return;

        // Check if delivered item matches the requested order
        if (order.CheckOrder(item.itemName))
        {
            Debug.Log("Correct item delivered: " + item.itemName);

            // Remove the delivered item
            Destroy(other.gameObject);

            // Serve the front NPC and shift the queue
            queue.ServeFrontNpc();
        }
        else
        {
            Debug.Log("Wrong item delivered: " + item.itemName);
        }
    }
}