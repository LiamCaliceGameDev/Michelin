using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Plate Rules/Must Contain Types Rule")]
public class MustContainTypesRule : PlateRule
{
    public List<Ingredient.IngredientType> requiredTypes = new List<Ingredient.IngredientType>();

    public override bool IsRuleSatisfied(PlateManager plateManager)
    {
        foreach (var type in requiredTypes)
        {
            bool hasType = plateManager.ingredientsOnPlate.Exists(i => i.ingredientData.ingredientType == type);
            if (!hasType)
                return false;
        }

        return true;
    }

    private void OnEnable()
    {
        ruleName = "Must contain: " + string.Join(", ", requiredTypes);
    }

    public override bool IsCompatibleWith(List<PlateRule> existingRules)
    {
        foreach (var rule in existingRules)
        {
            if (rule is MustNotContainTypesRule notContainRule)
            {
                // Check for conflict: if a type is both required and prohibited
                if (requiredTypes.Intersect(notContainRule.prohibitedTypes).Any())
                {
                    return false; // Conflict if same ingredient type is both required and prohibited
                }
            }
        }
        return base.IsCompatibleWith(existingRules);
    }


}
