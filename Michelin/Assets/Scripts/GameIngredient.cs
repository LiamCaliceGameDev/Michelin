using System.Collections.Generic;
using UnityEngine;

public class GameIngredient : MonoBehaviour
{
    [Header("Ingredient Data")]
    [Tooltip("The ScriptableObject containing this ingredient's data.")]
    public Ingredient ingredientData;

    [Header("State Flags")]
    public bool selected = false;
    public bool placed = false;
    public bool isBeingBinned;

    private Vector3 offset;
    private SpriteRenderer spriteRenderer;
    private static int sortingOrderCounter = 0;

    // Track connected ingredients via triggers
    public HashSet<GameIngredient> touchingIngredients = new HashSet<GameIngredient>();

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("No SpriteRenderer found on " + gameObject.name);
        }
        else
        {
            sortingOrderCounter++;
            spriteRenderer.sortingOrder = sortingOrderCounter;
        }
    }

    void OnMouseDown()
    {
        selected = true;
        placed = false;
        offset = transform.position - GetMouseWorldPosition();

        // Bring this ingredient to front
        if (spriteRenderer != null)
        {
            sortingOrderCounter++;
            spriteRenderer.sortingOrder = sortingOrderCounter;
        }
    }

    void Update()
    {
        if (selected)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }

        if (!placed)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (selected && PlateManager.instance.IsWithinPlate(transform.position))
                {
                    placed = true;
                    PlateManager.instance.RegisterIngredient(this);
                }
                else
                {
                    if (!isBeingBinned)
                    {
                        PlateManager.instance.UnRegisterIngrediennt(this);
                        Destroy(gameObject);
                    }
                }

                selected = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherIngredient = other.GetComponent<GameIngredient>();

        if (otherIngredient != null && otherIngredient != this)
        {
            touchingIngredients.Add(otherIngredient);
            otherIngredient.touchingIngredients.Add(this);
        }

        if (isBeingBinned && other.CompareTag("Bin"))
        {
            PlateManager.instance.UnRegisterIngrediennt(this);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var otherIngredient = other.GetComponent<GameIngredient>();
        if (otherIngredient != null)
        {
            touchingIngredients.Remove(otherIngredient);
            otherIngredient.touchingIngredients.Remove(this);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }
}
