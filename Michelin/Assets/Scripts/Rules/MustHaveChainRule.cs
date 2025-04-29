using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Plate Rules/Must Have Chain of Ingredients")]
public class MustHaveChainRule : PlateRule
{
    [Min(2)] public int chainSize = 3;

    private void OnEnable()
    {
        ruleName =  $"Chain {chainSize} touching ingredients";
    }

    public override bool IsRuleSatisfied(PlateManager plateManager)
    {
        return plateManager.GetLongestIngredientChain() >= chainSize;
    }

    private int GetChainLength(GameIngredient start, HashSet<GameIngredient> globalVisited, List<GameIngredient> allIngredients)
    {
        var queue = new Queue<GameIngredient>();
        var localVisited = new HashSet<GameIngredient>();

        queue.Enqueue(start);
        localVisited.Add(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            globalVisited.Add(current);

            foreach (var other in allIngredients)
            {
                if (other == current || localVisited.Contains(other)) continue;

                float distance = Vector2.Distance(current.transform.position, other.transform.position);
                float threshold = 0.6f; // tweak this if needed based on prefab size

                if (distance <= threshold)
                {
                    queue.Enqueue(other);
                    localVisited.Add(other);
                }
            }
        }

        return localVisited.Count;
    }

    public override bool IsCompatibleWith(List<PlateRule> existingRules)
    {
        foreach (var rule in existingRules)
        {
            // Incompatible with ingredient count rules
            if (rule is IngredientCountRule ingredientCountRule)
            {
                if (ingredientCountRule.requiredCount <= chainSize)
                    return false;
            }

            // Incompatible with other ChainRules
            if (rule is MustHaveChainRule)
                return false;

            // Incompatible with NoCollisionsRule
            if (rule is NoCollisionsRule)
                return false;
        }

        return true;
    }


}
