using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public float waterLevel = 0.0f; // The Y position of the water surface
    public float buoyancyForce = 10.0f; // The force applied when submerged
    public float damping = 0.5f; // Damping to reduce oscillation

    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component attached to this object
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Check if the object is below the water level
        if (transform.position.y < waterLevel)
        {
            // Calculate how much of the object is submerged
            float submergedDepth = waterLevel - transform.position.y;

            // Apply buoyancy force
            Vector3 force = Vector3.up * buoyancyForce * submergedDepth;
            rb.AddForce(force, ForceMode.Acceleration);

            // Apply damping to reduce oscillation
            rb.AddForce(-rb.velocity * damping, ForceMode.Acceleration);
        }
    }
}