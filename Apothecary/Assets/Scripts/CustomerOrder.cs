using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerOrder : MonoBehaviour
{
    [System.Serializable]
    public class Order
    {
        public string itemName;

        public string displayName;

        public Sprite icon;
    }

    public Order[] possibleOrders;

    public Order currentOrder;

    public Transform orderDisplayPoint;

    public TMP_Text orderText;
    public Image orderImage;

    private void Start()
    {
        ChooseRandomOrder();
    }

    void ChooseRandomOrder()
    {
        if (possibleOrders == null || possibleOrders.Length == 0) return;

        // Randomly choose one order from the list
        currentOrder = possibleOrders[Random.Range(0, possibleOrders.Length)];

        // Update text
        if (orderText != null)
            orderText.text = currentOrder.displayName;

        // Update the icon
        if (orderImage != null)
        {
            orderImage.sprite = currentOrder.icon;
            orderImage.enabled = currentOrder.icon != null;
        }
    }

    public bool CheckOrder(string deliveredItem)
    {
        // Return true only if the delivered item matches
        return currentOrder != null && deliveredItem == currentOrder.itemName;
    }
}