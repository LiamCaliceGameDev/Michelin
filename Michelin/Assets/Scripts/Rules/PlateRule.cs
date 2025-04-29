using System.Collections.Generic;
using UnityEngine;

public abstract class PlateRule : ScriptableObject
{
    public string ruleName;

    public abstract bool IsRuleSatisfied(PlateManager plateManager);

    public virtual bool IsCompatibleWith(List<PlateRule> existingRules)
    {
        // By default, rules are compatible with everything
        return true;
    }
}
