using UnityEngine;

public class EditMeshBounds : MonoBehaviour
{
    void Start()
    {
        // Get the MeshFilter component attached to this GameObject
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        
        if (meshFilter != null)
        {
            // Get the mesh from the MeshFilter
            Mesh mesh = meshFilter.mesh;

            // Modify the mesh bounds
            Vector3 newCenter = mesh.bounds.center;
            Vector3 newSize = mesh.bounds.size * 2; // Triple the size of the bounds

            // Set the new bounds
            mesh.bounds = new Bounds(newCenter, newSize);

            Debug.Log("Mesh bounds updated: " + mesh.bounds);
        }
        else
        {
            Debug.LogError("No MeshFilter found on this GameObject.");
        }
    }
}

