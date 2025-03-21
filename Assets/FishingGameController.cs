using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingGameController : MonoBehaviour
{
    public Transform fishMarker;  // Fish marker object (moves up and down)
    public Rigidbody2D hookRigidbody;    // Rigidbody2D for the HookArea (Hook)
    public Transform topPivot;    // Top boundary for fish movement
    public Transform bottomPivot; // Bottom boundary for fish movement
    public Slider progressBar;   // Progress bar slider
    public Canvas fishingCanvas;  // Canvas to show fishing UI
    public TextMeshProUGUI gameStatusText;   // TextMeshPro component for game status (Win/Loss)
    public TextMeshProUGUI timerText;  // TextMeshPro component to show the timer countdown

    public float fishSpeed = 0.05f;  // Speed of fish movement up and down (slower movement)
    public float hookJumpForce = 60f;    // Upward force applied when space is pressed (Increased for faster jump)
    public float gravityScale = 10f; // Gravity scale for Rigidbody2D (Increased for quicker fall)

    private bool isFishingActive = false;  // Flag to check if the fishing game is active
    private bool isHookAboveFish = false; // Flag to check if hook is above the fish
    private float currentProgress = 0f;   // Current progress on the progress bar
    private float remainingTime = 20f; // Timer that counts down from 20 seconds

    private Vector2 fishMovementDirection; // Direction for random fish movement

    public float collisionTolerance = 0.2f; // Tolerance for collision (when the hook is near the fish)

    void Start()
    {
        // Null checks and debugging for missing references
        if (fishingCanvas == null)
        {
            Debug.LogError("Fishing Canvas is not assigned!");
        }

        if (gameStatusText == null)
        {
            Debug.LogError("Game Status Text is not assigned!");
        }

        if (timerText == null)
        {
            Debug.LogError("Timer Text is not assigned!");
        }

        if (progressBar == null)
        {
            Debug.LogError("Progress Bar is not assigned!");
        }

        // Hide canvas initially and reset values
        fishingCanvas.gameObject.SetActive(false); // Hide the fishing canvas initially
        progressBar.value = 0f;  // Set the slider's value to 0 at the start
        currentProgress = 0f;    // Reset progress
        remainingTime = 20f; // Set the timer to 20 seconds
        
        // Ensure Rigidbody2D has gravity scale set correctly
        hookRigidbody.gravityScale = gravityScale;
        hookRigidbody.velocity = Vector2.zero;  // Start with no velocity

        // Set initial random fish movement direction
        fishMovementDirection = RandomDirection();
    }

    void Update()
    {
        if (isFishingActive)
        {
            // Move the fish marker randomly within bounds
            MoveFishMarker();

            // Handle hook movement using spacebar input
            MoveHook();

            // Check if the hook is over the fish and update progress bar
            CheckIfHookIsOnFish();

            // Update progress bar
            UpdateProgressBar();

            // Update the timer countdown
            UpdateTimer();
        }
    }

    public void StartFishingGame()
    {
        if (isFishingActive) return;  // Prevent starting the game multiple times

        isFishingActive = true;  // Start the fishing game
        progressBar.value = 0f;  // Reset progress bar value
        currentProgress = 0f;    // Reset current progress
        remainingTime = 20f; // Reset the timer to 20 seconds
        fishingCanvas.gameObject.SetActive(true);  // Show the fishing UI
        gameStatusText.text = "Catch the Fish!";   // Show instructions text

        Debug.Log("Fishing game started!"); // Debugging message to confirm game started
    }

    void MoveFishMarker()
    {
        // Slowly move the fish in a randomized pattern within the bounds
        float fishPosition = Mathf.PingPong(Time.time * fishSpeed, 1f); // Moves up and down between 0 and 1
        fishMarker.position = Vector3.Lerp(topPivot.position, bottomPivot.position, fishPosition);  // Move the fish marker within the pivot bounds

        // Apply random vertical direction to fish movement
        if (Random.Range(0f, 1f) < 0.01f)  // Change direction randomly
        {
            fishMovementDirection = RandomDirection();
        }
        
        // Apply random movement to fish within the vertical bounds
        fishMarker.position += new Vector3(0, fishMovementDirection.y * Time.deltaTime * fishSpeed, 0);
    }

    // Get a random direction for the fish to move in
    Vector2 RandomDirection()
    {
        return new Vector2(0, Random.Range(-0.2f, 0.2f));  // Randomize the fish movement direction slightly up or down
    }

    void MoveHook()
    {
        // Move the hook upwards when space is pressed, otherwise let gravity pull it down
        if (Input.GetKey(KeyCode.Space))  // Apply an initial force when space is pressed
        {
            hookRigidbody.velocity = Vector2.zero;  // Reset any previous velocity to avoid stacking forces
            hookRigidbody.AddForce(Vector2.up * hookJumpForce, ForceMode2D.Impulse);  // Apply an upward force (jump)
        }
        else
        {
            // Apply downward force (gravity) when space is not pressed, making the hook fall back down
            hookRigidbody.velocity = new Vector2(hookRigidbody.velocity.x, Mathf.Clamp(hookRigidbody.velocity.y, -100f, float.MaxValue));
        }

        // Restrict the hook's vertical movement to stay between top and bottom pivots
        float clampedY = Mathf.Clamp(hookRigidbody.position.y, bottomPivot.position.y, topPivot.position.y);
        hookRigidbody.position = new Vector2(hookRigidbody.position.x, clampedY);
    }

    void CheckIfHookIsOnFish()
    {
        // Check if the hook is near the fish within a tolerance range
        if (Mathf.Abs(hookRigidbody.position.y - fishMarker.position.y) < collisionTolerance)
        {
            isHookAboveFish = true; // Hook is above the fish within tolerance
        }
        else
        {
            isHookAboveFish = false; // Hook is not over the fish
        }
    }

    void UpdateProgressBar()
    {
        // Slow down the progress bar updates for a smoother experience
        if (isHookAboveFish)
        {
            currentProgress += Time.deltaTime * 0.1f;  // Slow progress increase when hooked
        }
        else
        {
            currentProgress -= Time.deltaTime * 0.05f;  // Slow progress decrease when hook isn't over fish
        }

        // Clamp progress to ensure it doesn't go below 0 or above 1
        currentProgress = Mathf.Clamp(currentProgress, 0f, 1f);

        // Update the progress bar slider's value
        progressBar.value = currentProgress;

        // Check if the player has caught the fish (progress bar reaches 100%)
        if (currentProgress >= 1f)
        {
            EndFishingGame(true);  // End the game with a win
        }
    }

    void UpdateTimer()
    {
        // Null check for timerText (in case it's not assigned)
        if (timerText == null)
        {
            Debug.LogError("Timer Text is null!");
            return;
        }

        // Decrease the timer by the time passed
        remainingTime -= Time.deltaTime;

        // Update the timer text
        timerText.text = "Time Left: " + Mathf.Ceil(remainingTime).ToString();

        // If time runs out and progress is not full, player loses
        if (remainingTime <= 0f)
        {
            if (currentProgress < 1f)
            {
                EndFishingGame(false);  // Player loses the fish
            }
        }
    }

    void EndFishingGame(bool isWin)
    {
        isFishingActive = false;  // Stop the fishing game
        fishingCanvas.gameObject.SetActive(false);  // Hide the fishing UI

        // Show the win/loss message
        if (isWin)
        {
            gameStatusText.text = "You Caught the Fish!";
        }
        else
        {
            gameStatusText.text = "You Lost the Fish!";
        }

        Debug.Log(isWin ? "Fishing game ended: Win!" : "Fishing game ended: Lose!");
    }
}
