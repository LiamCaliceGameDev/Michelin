using UnityEngine;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
{
    [Header("Ingredient Data")]
    public Ingredient ingredientData;

    [Header("UI Components")]
    public Image iconImage;

    void Start()
    {
        if (ingredientData != null && iconImage != null)
        {
            iconImage.sprite = ingredientData.icon;
        }
    }

    // 👇 Call this from the EventTrigger's PointerDown event
    public void SpawnIngredient()
    {
        if (ingredientData == null || ingredientData.ingredientPrefab == null)
        {
            Debug.LogWarning("Missing ingredient data or prefab.");
            return;
        }

        // Instantiate at mouse position
        Vector3 spawnPosition = GetMouseWorldPosition();
        GameObject newObj = Instantiate(ingredientData.ingredientPrefab, spawnPosition, Quaternion.identity);

        // Set it as selected to drag
        GameIngredient gameIngredient = newObj.GetComponent<GameIngredient>();
        if (gameIngredient != null)
        {
            gameIngredient.selected = true;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 1f; // Adjust as needed
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
