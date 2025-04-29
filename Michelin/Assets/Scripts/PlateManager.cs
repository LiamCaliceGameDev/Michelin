using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Ingredient;

public class PlateManager : MonoBehaviour
{

    [Header("Rule System")]
    public List<PlateRule> currentRules = new List<PlateRule>();
    public List<PlateRule> availableRules; // Assign via Inspector (add your rule assets here)
    public int minRules = 1;
    public int maxRules = 1;

    public RuleUIManager ruleUIManager; // Assign via Inspector


    [Header("Plate Settings")]
    public Vector3 plateCenter = Vector3.zero;
    public float plateRadius = 1.0f;

    [Header("Gizmo Settings")]
    public Color gizmoColor = Color.green;

    public static PlateManager instance;

    // List of ingredients currently on the plate
    public List<GameIngredient> ingredientsOnPlate = new List<GameIngredient>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        NewPlate();
    }
    public void NewPlate()
    {
        ResetPlate();
        currentRules.Clear();

        int ruleCount = Random.Range(minRules, maxRules + 1);
        for (int i = 0; i < ruleCount; i++)
        {
            PlateRule rule = availableRules[Random.Range(0, availableRules.Count)];
            currentRules.Add(rule);
        }

        ruleUIManager?.DisplayRules(currentRules);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position + plateCenter, plateRadius);
    }

    /// <summary>
    /// Check if a transform is within the plate area.
    /// </summary>
    public bool IsWithinPlate(Transform objTransform)
    {
        return IsWithinPlate(objTransform.position);
    }

    /// <summary>
    /// Check if a world position is within the plate area.
    /// </summary>
    public bool IsWithinPlate(Vector2 position)
    {
        float distance = Vector2.Distance(position, transform.position + plateCenter);
        return distance <= plateRadius;
    }

    /// <summary>
    /// Call this when an ingredient is placed on the plate.
    /// </summary>
    public void RegisterIngredient(GameIngredient ingredient)
    {
        if (!ingredientsOnPlate.Contains(ingredient))
        {
            ingredientsOnPlate.Add(ingredient);
        }
    }

    public void UnRegisterIngrediennt(GameIngredient ingredient)
    {
        if (ingredientsOnPlate.Contains(ingredient))
        {
            ingredientsOnPlate.Remove(ingredient);
        }
    }

    /// <summary>
    /// Call this when an ingredient is removed/destroyed.
    /// </summary>
    public void UnregisterIngredient(GameIngredient ingredient)
    {
        ingredientsOnPlate.Remove(ingredient);
    }

    public int GetTotalIngredients()
    {
        return ingredientsOnPlate.Count;
    }

    public float GetTotalWeight()
    {
        return ingredientsOnPlate.Sum(i => i.ingredientData.weight);
    }

    public int GetIngredientCountByType(IngredientType type)
    {
        return ingredientsOnPlate.Count(i => i.ingredientData.ingredientType == type);
    }

    public void ResetPlate()
    {
        // Create a copy to avoid modifying the list while iterating
        var ingredientsToRemove = new List<GameIngredient>(ingredientsOnPlate);

        foreach (var ingredient in ingredientsToRemove)
        {
            if (ingredient != null)
            {
                Destroy(ingredient.gameObject);
            }
        }

        ingredientsOnPlate.Clear();
    }

    public void SendOutPlate()
    {
        bool allRulesSatisfied = currentRules.All(rule => rule.IsRuleSatisfied(this));

        if (allRulesSatisfied)
        {
            Debug.Log("✅ Success! All rules are satisfied.");
        }
        else
        {
            Debug.Log("❌ Wrong! Some rules are not satisfied.");
        }

        NewPlate();

    }


}
