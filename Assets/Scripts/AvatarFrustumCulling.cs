using UnityEngine;

public class AvatarFrustumCulling : MonoBehaviour
{
    public Vector3 newBoundsCenter = Vector3.zero;
    public Vector3 newBoundsSize = Vector3.one * 10000f;

    void Start()
    {
        AdjustBoundingBox();
    }

    void AdjustBoundingBox()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.bounds = new Bounds(newBoundsCenter, newBoundsSize);
            Debug.Log("Adjusted bounding box of renderer: " + renderer.name);
        }
        else
        {
            Debug.LogWarning("No Renderer component found on the target object.");
        }
    }
}
