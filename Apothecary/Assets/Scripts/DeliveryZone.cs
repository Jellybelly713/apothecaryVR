using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [SerializeField] private NpcQueueManager queue;

    [SerializeField] private FeedbackUI feedbackUI;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctDeliveryClip;
    [SerializeField] private AudioClip wrongDeliveryClip;

    private void OnTriggerEnter(Collider other)
    {
        IngredientItem item = other.GetComponentInParent<IngredientItem>();
        if (item == null) return;

        // Get the customer at the front of the queue
        NpcQueueMember frontNpc = queue != null ? queue.FrontNpc : null;
        if (frontNpc == null)
        {
            if (feedbackUI != null)
                feedbackUI.ShowMessage("No customer at the window");
            return;
        }

        // Get that customers current order
        CustomerOrder order = frontNpc.GetComponent<CustomerOrder>();
        if (order == null)
        {
            if (feedbackUI != null)
                feedbackUI.ShowMessage("Customer has no order");
            return;
        }

        // Check whether the delivered item matches
        if (order.CheckOrder(item.itemId))
        {
            if (feedbackUI != null)
                feedbackUI.ShowMessage("Correct order delivered!");

            if (audioSource != null && correctDeliveryClip != null)
                audioSource.PlayOneShot(correctDeliveryClip);

            // Remove the delivered item from the scene
            Destroy(other.gameObject);

            // shift the costumer queue
            queue.ServeFrontNpc();
        }
        else
        {
            if (feedbackUI != null)
                feedbackUI.ShowMessage("Wrong order");

            if (audioSource != null && wrongDeliveryClip != null)
                audioSource.PlayOneShot(wrongDeliveryClip);
        }
    }
}