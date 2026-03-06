using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    [System.Serializable]
    public class Order
    {
        public string itemName;
        public GameObject visualPrefab; //to show above head
    }

    public Order[] possibleOrders;
    public Order currentOrder;

    public Transform orderDisplayPoint; // where the icon/text appears

    private void Start()
    {
        ChooseRandomOrder();
    }

    void ChooseRandomOrder()
    {
        if (possibleOrders.Length == 0) return;
        currentOrder = possibleOrders[Random.Range(0, possibleOrders.Length)];

        // spawn a floating visual above their head here
        if (currentOrder.visualPrefab && orderDisplayPoint)
            Instantiate(currentOrder.visualPrefab, orderDisplayPoint.position, Quaternion.identity, orderDisplayPoint);
    }

    public bool CheckOrder(string deliveredItem)
    {
        return deliveredItem == currentOrder.itemName;
    }
}
