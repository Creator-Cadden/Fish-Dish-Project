using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    public float gravityScale = 0.5f; // Adjust this value to change the gravity effect
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Apply custom gravity
        Vector3 customGravity = new Vector3(0, Physics.gravity.y * gravityScale, 0);
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }
}

