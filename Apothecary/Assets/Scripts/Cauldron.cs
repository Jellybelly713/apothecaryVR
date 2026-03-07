using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour, Interactable
{
    [System.Serializable]
    public class Recipe
    {
        public string recipeName;
        public List<string> ingredientIds = new List<string>();
        public GameObject resultPrefab;
        public string resultDisplayName;
    }

    public List<Recipe> recipes = new List<Recipe>();
    public List<string> currentIngredients = new List<string>();
    public Transform resultSpawnPoint;
    public FeedbackUI feedbackUI;

    [SerializeField] private int maxIngredients = 5;

    // Audio setup
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ingredientAddedClip;
    [SerializeField] private AudioClip recipeSuccessClip;
    [SerializeField] private AudioClip recipeFailClip;

    public void AddIngredient(IngredientItem item)
    {
        if (item == null) return;

        // Stop adding ingredients if the cauldron is full
        if (currentIngredients.Count >= maxIngredients)
        {
            if (feedbackUI != null)
                feedbackUI.ShowMessage("Cauldron is full.");
            return;
        }

        // Store the item ID
        currentIngredients.Add(item.itemId);

        // Tell the player what got added
        if (feedbackUI != null)
            feedbackUI.ShowMessage(item.displayName + " added to cauldron.");

        // Play the ingredient added sound
        if (audioSource != null && ingredientAddedClip != null)
            audioSource.PlayOneShot(ingredientAddedClip);

        Destroy(item.gameObject);
    }

    public void Interact()
    {
        Brew();
    }

    public void Brew()
    {
        // Don't brew if nothing has been added
        if (currentIngredients.Count == 0)
        {
            if (feedbackUI != null)
                feedbackUI.ShowMessage("Cauldron is empty.");
            return;
        }

        Recipe matchedRecipe = FindMatchingRecipe();

        // If nothing matches, fail the brew and clear the ingredients
        if (matchedRecipe == null)
        {
            if (feedbackUI != null)
                feedbackUI.ShowMessage("Recipe failed.");

            if (audioSource != null && recipeFailClip != null)
                audioSource.PlayOneShot(recipeFailClip);

            currentIngredients.Clear();
            return;
        }

        // Spawn brew if the recipe has a valid prefab and spawn point
        if (matchedRecipe.resultPrefab != null && resultSpawnPoint != null)
        {
            Instantiate(matchedRecipe.resultPrefab, resultSpawnPoint.position, resultSpawnPoint.rotation);
        }

        // feedback
        if (feedbackUI != null)
            feedbackUI.ShowMessage("Brewed: " + matchedRecipe.resultDisplayName);

        if (audioSource != null && recipeSuccessClip != null)
            audioSource.PlayOneShot(recipeSuccessClip);

        // Clear the ingredient list so the next brew starts fresh
        currentIngredients.Clear();
    }

    private Recipe FindMatchingRecipe()
    {
        // return first recipe that matches
        foreach (Recipe recipe in recipes)
        {
            if (RecipeMatches(recipe))
                return recipe;
        }

        return null;
    }

    private bool RecipeMatches(Recipe recipe)
    {
        if (recipe.ingredientIds.Count != currentIngredients.Count)
            return false;

        List<string> temp = new List<string>(currentIngredients);

        foreach (string needed in recipe.ingredientIds)
        {
            // If ingredient is missing, recipe fails
            if (!temp.Contains(needed))
                return false;

            temp.Remove(needed);
        }

        return true;
    }
}