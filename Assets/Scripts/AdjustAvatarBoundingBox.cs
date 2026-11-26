using UnityEngine;

public class AdjustAvatarBoundingBox : MonoBehaviour
{
    public Vector3 newCenter = Vector3.zero;
    public Vector3 newSize = Vector3.one;

    void Start()
    {
        AdjustBoundingBox();
    }

    void AdjustBoundingBox()
    { 
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;
            AdjustMeshBoundingBox(mesh);
        }

        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            Mesh mesh = skinnedMeshRenderer.sharedMesh;
            AdjustMeshBoundingBox(mesh);
        }
    }

    void AdjustMeshBoundingBox(Mesh mesh)
    {
        if (mesh != null)
        {
            mesh.bounds = new Bounds(newCenter, newSize);
            Debug.Log("Adjusted bounding box of mesh: " + mesh.name);
        }
        else
        {
            Debug.LogWarning("No mesh found to adjust bounding box.");
        }
    }
}

