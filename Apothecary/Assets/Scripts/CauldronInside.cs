using UnityEngine;

public class CauldronInside : MonoBehaviour
{
    [SerializeField] private Cauldron cauldron;

    private void OnTriggerEnter(Collider other)
    {
        // find an IngredientItem on the object or one of its parents
        IngredientItem item = other.GetComponentInParent<IngredientItem>();
        if (item == null) return;

        if (cauldron != null)
            cauldron.AddIngredient(item);
    }
}