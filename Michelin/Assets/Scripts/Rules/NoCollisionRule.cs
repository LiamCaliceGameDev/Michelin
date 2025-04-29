using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Plate Rules/NoCollisionsRule")]
public class NoCollisionsRule : PlateRule
{
    public override bool IsRuleSatisfied(PlateManager plateManager)
    {
        foreach (var ingredient in plateManager.ingredientsOnPlate)
        {
            if (ingredient.touchingIngredients.Count > 0)
                return false;
        }

        return true;
    }

    public void OnEnable()
    {
        ruleName = "No ingredients should be touching each other.";
    }

    public override bool IsCompatibleWith(List<PlateRule> existingRules)
    {
        // Cannot coexist with chain rule
        foreach (var rule in existingRules)
        {
            if (rule is MustHaveChainRule)
                return false;
        }
        return true;
    }
}
