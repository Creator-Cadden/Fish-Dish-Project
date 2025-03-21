using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 5f;
    public float groundDist;
    public LayerMask terrainLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;
    public CinemachineCamera CMcamera;

    private BoatController boat;
    private bool isOnBoat = false;
    private bool isInBoatArea = false;

    private Buoyancy playerBuoyancy; // Reference to the Buoyancy script
    private CustomGravity playerCustomGravity; // Reference to the CustomGravity script

    // Reference to FishingGameController
    public FishingGameController fishingGameController;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerBuoyancy = GetComponent<Buoyancy>(); // Get the Buoyancy script
        playerCustomGravity = GetComponent<CustomGravity>(); // Get the CustomGravity script
    }

    private void Update()
    {
        if (isOnBoat)
        {
            FollowBoatCamera();
            boat.MoveBoat(); // Move the boat
            return;
        }
        else
        {
            HandleMovement();
            HandleInteraction();
            HandleFishingGame();
        }
    }

    private void HandleInteraction()
    {
        // Check if the player is in the boat area and presses E to enter the boat
        if (isInBoatArea && Input.GetKeyDown(KeyCode.E) && !isOnBoat)
        {
            Debug.Log("E key pressed, attempting to enter the boat");
            EnterBoat();
        }
        // Check if the player is on the boat and presses E to exit
        else if (isOnBoat && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed, attempting to exit the boat");
            ExitBoat();
        }
    }

    private void HandleMovement()
    {
        RaycastHit hit;
        Vector3 castPos = transform.position;
        castPos.y += 1;
        if (Physics.Raycast(castPos, -transform.up, out hit, Mathf.Infinity, terrainLayer))
        {
            if (hit.collider != null)
            {
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDist;
                transform.position = movePos;
            }
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(x, 0f, y);

        rb.velocity = new Vector3(moveDir.x * playerSpeed, rb.velocity.y, moveDir.z * playerSpeed);

        if (x != 0)
        {
            sr.flipX = x < 0;
        }
    }

    private void EnterBoat()
    {
        // When entering the boat
        Debug.Log("Player is entering the boat.");
        isOnBoat = true;
        rb.isKinematic = true; // Disable physics for the player while on the boat

        // Disable Buoyancy and CustomGravity scripts
        if (playerBuoyancy != null)
            playerBuoyancy.enabled = false;
        if (playerCustomGravity != null)
            playerCustomGravity.enabled = false;

        boat.LockPlayerOnBoat(this); // Inform the boat that the player has boarded
    }

    public void ExitBoat()
    {
        // When exiting the boat
        Debug.Log("Player is exiting the boat.");
        isOnBoat = false;
        rb.isKinematic = false;

        // Re-enable Buoyancy and CustomGravity scripts
        if (playerBuoyancy != null)
            playerBuoyancy.enabled = true;
        if (playerCustomGravity != null)
            playerCustomGravity.enabled = true;

        boat.UnlockPlayer();
        // Place the player next to the boat when they exit
        transform.position = boat.transform.position + new Vector3(2f, 0f, 2f); // Adjust position as needed
    }

    private void FollowBoatCamera()
    {
        if (CMcamera != null)
        {
            Vector3 cameraPosition = boat.transform.position - boat.transform.forward * 10f + Vector3.up * 5f;
            CMcamera.transform.position = Vector3.Lerp(CMcamera.transform.position, cameraPosition, Time.deltaTime * 5f);
            CMcamera.transform.LookAt(boat.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if player entered boat area
        if (other.CompareTag("Boat"))
        {
            isInBoatArea = true;
            boat = other.GetComponent<BoatController>();
            Debug.Log("Entered boat area. Boat reference: " + (boat != null));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if player exited boat area
        if (other.CompareTag("Boat"))
        {
            isInBoatArea = false;
            boat = null;
            Debug.Log("Exited boat area");
        }
    }

    private void HandleFishingGame()
    {
        // Check if the player is in the correct area to fish and presses F to start fishing
        if (isInBoatArea && Input.GetKeyDown(KeyCode.F))
        {
            // Start the fishing mini-game
            if (fishingGameController != null)
            {
                fishingGameController.StartFishingGame();
                Debug.Log("Fishing game started!");
            }
        }
    }
}
