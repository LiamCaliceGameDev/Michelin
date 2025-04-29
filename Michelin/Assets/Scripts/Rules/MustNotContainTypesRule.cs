using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Plate Rules/Must Not Contain Types Rule")]
public class MustNotContainTypesRule : PlateRule
{
    public List<Ingredient.IngredientType> prohibitedTypes = new List<Ingredient.IngredientType>();

    // A reference to the list of all possible ingredient types in your game
    public List<Ingredient.IngredientType> allIngredientTypes;

    public override bool IsRuleSatisfied(PlateManager plateManager)
    {
        foreach (var type in prohibitedTypes)
        {
            bool hasType = plateManager.ingredientsOnPlate.Exists(i => i.ingredientData.ingredientType == type);
            if (hasType)
                return false;
        }

        return true;
    }

    private void OnEnable()
    {
        ruleName = "Must not contain: " + string.Join(", ", prohibitedTypes);
    }

    public override bool IsCompatibleWith(List<PlateRule> existingRules)
    {
        foreach (var rule in existingRules)
        {
            if (rule is MustContainTypesRule containRule)
            {
                // Check for conflict: if a type is both required and prohibited
                if (containRule.requiredTypes.Intersect(prohibitedTypes).Any())
                {
                    return false; // Conflict if same ingredient type is both required and prohibited
                }
            }
        }
        return base.IsCompatibleWith(existingRules);
    }

    public bool IsValid()
    {
        // Prevent prohibiting all types
        if (prohibitedTypes.Count == allIngredientTypes.Count)
        {
            Debug.LogError("Cannot prohibit all ingredient types!");
            return false;
        }

        return true;
    }
}
