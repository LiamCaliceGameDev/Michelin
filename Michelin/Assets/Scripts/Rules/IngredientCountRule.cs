using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Plate Rules/Ingredient Count Rule")]
public class IngredientCountRule : PlateRule
{
    public int requiredCount;

    public override bool IsRuleSatisfied(PlateManager plateManager)
    {
        return plateManager.GetTotalIngredients() == requiredCount;
    }

    private void OnEnable()
    {
        ruleName = $"Place exactly {requiredCount} ingredient(s)";
    }

    public override bool IsCompatibleWith(List<PlateRule> existingRules)
    {
        foreach (var rule in existingRules)
        {
            if (rule is IngredientCountRule)
            {
                return false; // Only one ingredient count rule allowed
            }
        }
        return true;
    }

}
