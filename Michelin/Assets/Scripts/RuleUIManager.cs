using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuleUIManager : MonoBehaviour
{
    public GameObject ruleTextPrefab; // A prefab with a Text component
    public Transform ruleListParent;  // Attach to Vertical Layout Group

    public void DisplayRules(List<PlateRule> rules)
    {
        foreach (Transform child in ruleListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var rule in rules)
        {
            GameObject textObj = Instantiate(ruleTextPrefab, ruleListParent);
            textObj.GetComponent<TMP_Text>().text = rule.ruleName;
        }
    }
}
