using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Cooking/Ingredient")]
public class Ingredient : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("Name of the ingredient (e.g., Salmon, Basil, Carrot).")]
    public string ingredientName;

    [Tooltip("Weight of the ingredient in kilograms.")]
    public float weight;

    [Tooltip("Type of the ingredient (Fish, Meat, Vegetable, Herb).")]
    public IngredientType ingredientType;

    [Header("Visual & Prefab")]
    [Tooltip("Icon representing the ingredient.")]
    public Sprite icon;

    [Tooltip("Prefab to instantiate this ingredient in the scene.")]
    public GameObject ingredientPrefab;

    public enum IngredientType
    {
        Fish,
        Meat,
        Vegetable,
        Herb
    }
}
