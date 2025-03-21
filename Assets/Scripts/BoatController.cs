using UnityEngine;

public class BoatController : MonoBehaviour
{
    // Public variables to control movement
    public float forwardSpeed = 8f;
    public float turnSpeed = 5f;
    public float driftFactor = 0.95f; // How much drift is allowed
    public float driftStrength = 3f; // How strong the drift effect is
    public float lerpSpeed = 8f; // Lerp speed for smooth deceleration and drift reduction
    public float maxSpeed = 10f; // Maximum speed of the boat
    public float maxTurnSpeed = 10f; // Maximum turning speed

    // Internal variables
    private Rigidbody rb;
    private PlayerController currentPlayer;  // The player currently on the boat

    // Buoyancy variables
    public float waterLevel = 0.0f; // The Y position of the water surface
    public float buoyancyForce = 10.0f; // The force applied when submerged
    public float damping = 0.5f; // Damping to reduce oscillation

    // Custom gravity variables
    public float gravityScale = 0.5f; // Adjust this value to change the gravity effect

    // Variables for drift handling
    private float currentTurnSpeed = 0f;
    private float horizontalInput = 0f;

    void Start()
    {
        // Get the Rigidbody component attached to the boat
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero; // Ensure the center of mass is at the boat's center

        // Configure Rigidbody for stable movement
        rb.mass = 100f; // Adjust mass for realistic boat behavior
        rb.drag = 1f; // Increase drag to slow down the boat naturally
        rb.angularDrag = 10f; // Increase angular drag to prevent excessive spinning
    }

    void FixedUpdate()
    {
        // If there's a player on the boat, we should apply boat movement controls
        if (currentPlayer != null)
        {
            // Handle drifting, turning, and movement with the boat's physics
            MoveBoat();
            ApplyDrift();
            SmoothDrift();
        }

        // Apply buoyancy and custom gravity for the boat
        ApplyBuoyancy();
        ApplyCustomGravity();
    }

    // New MoveBoat method for external use
    public void MoveBoat()
    {
        float moveInput = Input.GetAxis("Vertical");  // W/S or Up/Down arrows for forward/backward movement
        horizontalInput = Input.GetAxis("Horizontal");  // A/D or Left/Right arrows for turning

        // Apply forward movement
        Vector3 forwardMovement = transform.forward * moveInput * forwardSpeed * Time.fixedDeltaTime;
        rb.AddForce(forwardMovement, ForceMode.VelocityChange);

        // Apply turning
        float turnAmount = horizontalInput * turnSpeed * Time.fixedDeltaTime;
        rb.AddTorque(Vector3.up * turnAmount, ForceMode.VelocityChange);

        // Limit the boat's speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Limit the boat's turning speed
        if (rb.angularVelocity.magnitude > maxTurnSpeed)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxTurnSpeed;
        }
    }

    void ApplyDrift()
    {
        // Drift happens by modifying the velocity over time to simulate boat-like behavior
        Vector3 currentVelocity = rb.velocity;
        Vector3 driftDirection = Vector3.Cross(currentVelocity, transform.up).normalized;

        // Apply drift effect to the boat
        Vector3 driftForce = driftDirection * driftStrength * currentVelocity.magnitude;
        rb.AddForce(driftForce, ForceMode.VelocityChange);
    }

    void SmoothDrift()
    {
        // Reduce drifting and smooth the boat's movements
        Vector3 velocity = rb.velocity;

        // Apply a controlled drift based on horizontal turning input
        if (Mathf.Abs(horizontalInput) > 0.2f && Mathf.Abs(velocity.magnitude) > 0.2f)
        {
            // Apply sideways drag to simulate drifting, but in a more controlled way
            rb.velocity = new Vector3(velocity.x * (1 - driftFactor), velocity.y, velocity.z);
        }

        // Reduce any drift by smoothly blending the sideways velocity towards zero
        velocity = Vector3.Lerp(velocity, new Vector3(velocity.x * driftFactor, velocity.y, velocity.z * driftFactor), Time.fixedDeltaTime * lerpSpeed);

        // Apply the smoothed velocity back to the rigidbody
        rb.velocity = velocity;
    }

    void ApplyBuoyancy()
    {
        // Check if the boat is below the water level
        if (transform.position.y < waterLevel)
        {
            // Calculate how much of the boat is submerged
            float submergedDepth = waterLevel - transform.position.y;

            // Apply buoyancy force
            Vector3 force = Vector3.up * buoyancyForce * submergedDepth;
            rb.AddForce(force, ForceMode.Acceleration);

            // Apply damping to reduce oscillation
            rb.AddForce(-rb.velocity * damping, ForceMode.Acceleration);
        }
    }

    void ApplyCustomGravity()
    {
        // Apply custom gravity
        Vector3 customGravity = new Vector3(0, Physics.gravity.y * gravityScale, 0);
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }

    public void LockPlayerOnBoat(PlayerController player)
    {
        currentPlayer = player;  // Store the reference to the player
        player.transform.SetParent(transform);  // Parent the player to the boat so it moves with it
        player.transform.localPosition = new Vector3(0f, 1.5f, 0f);  // Position the player above the boat
        player.GetComponent<Rigidbody>().isKinematic = true;  // Disable the player's physics while on the boat
    }

    public void UnlockPlayer()
    {
        if (currentPlayer != null)
        {
            currentPlayer.transform.SetParent(null);  // Remove the player from the boat's parent
            currentPlayer.GetComponent<Rigidbody>().isKinematic = false;  // Re-enable the player's physics
            currentPlayer = null;  // Clear the reference to the player
        }
    }
}