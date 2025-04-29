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
}
