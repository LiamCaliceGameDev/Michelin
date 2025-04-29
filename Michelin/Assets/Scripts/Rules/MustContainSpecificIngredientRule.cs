using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Plate Rules/Must Contain Specific Ingredient")]
public class MustContainSpecificIngredientRule : PlateRule
{
    public Ingredient requiredIngredient; // Assign via inspector

    public override bool IsRuleSatisfied(PlateManager plateManager)
    {
        return plateManager.ingredientsOnPlate.Exists(i => i.ingredientData == requiredIngredient);
    }


    private void OnEnable()
    {
        if (requiredIngredient != null) {
            ruleName = $"Must contain: {requiredIngredient.name}";
        }
    }

    public override bool IsCompatibleWith(List<PlateRule> existingRules)
    {
        // Prevent conflicts with rules that ban the ingredient's type
        foreach (var rule in existingRules)
        {
            if (rule is MustNotContainTypesRule notContainRule)
            {
                if (notContainRule.prohibitedTypes.Contains(requiredIngredient.ingredientType))
                {
                    return false; // Conflict: e.g. must have Tomato (Vegetable) but must not have Vegetable
                }
            }

            if (rule is MustContainSpecificIngredientRule specificRule)
            {
                if (specificRule.requiredIngredient == requiredIngredient)
                {
                    return false; // Avoid duplicate specific rules
                }
            }
        }

        return true;
    }
}
