using UnityEngine;

public abstract class PlateRule : ScriptableObject
{
    public string ruleName;
    public abstract bool IsRuleSatisfied(PlateManager plateManager);
}
