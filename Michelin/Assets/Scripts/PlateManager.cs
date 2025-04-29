using UnityEngine;

public class PlateManager : MonoBehaviour
{
    [Header("Plate Settings")]
    public Vector3 plateCenter = Vector3.zero;
    public float plateRadius = 1.0f;

    [Header("Gizmo Settings")]
    public Color gizmoColor = Color.green;

    public static PlateManager instance;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Check if a transform is within the plate area.
    /// </summary>
    public bool IsWithinPlate(Transform objTransform)
    {
        return IsWithinPlate(objTransform.position);
    }

    /// <summary>
    /// Check if a world position is within the plate area.
    /// </summary>
    public bool IsWithinPlate(Vector2 position)
    {
        float distance = Vector2.Distance(position, transform.position + plateCenter);
        return distance <= plateRadius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position + plateCenter, plateRadius);
    }
}
