using UnityEngine;

public class GameIngredient : MonoBehaviour
{
    [Header("Ingredient Data")]
    [Tooltip("The ScriptableObject containing this ingredient's data.")]
    public Ingredient ingredientData;

    [Header("State Flags")]
    public bool selected = false;
    public bool placed = false;

    private Vector3 offset;

    public bool isBeingBinned;

    void OnMouseDown()
    {
        selected = true;
        placed = false;
        offset = transform.position - GetMouseWorldPosition();
    }

    void Update()
    {
        if (selected)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }

        if (!placed) {
            if (Input.GetMouseButtonUp(0))
            {
                if (selected && PlateManager.instance.IsWithinPlate(transform.position))
                {
                  
                    placed = true;
                }
                else
                {
                    if (!isBeingBinned)
                    {
                        Destroy(gameObject);
                    }
                   
                }

                selected = false;
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isBeingBinned)
        {
            if (other.CompareTag("Bin"))
            {
                Destroy(gameObject);
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }
}
