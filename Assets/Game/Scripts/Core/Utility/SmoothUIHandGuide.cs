using UnityEngine;
using System.Collections;

// This component ensures that the GameObject must have a RectTransform.
[RequireComponent(typeof(RectTransform))]
public class SmoothUIHandGuide : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The direction and distance the hand will move from its starting point.")]
    public Vector2 movementVector = new Vector2(100f, 0f);

    [Tooltip("The time in seconds it takes to travel from the start to the end point.")]
    [Min(0.1f)]
    public float durationOneWay = 1.5f;

    [Tooltip("Delay in seconds before the movement starts after the object is enabled.")]
    [Min(0f)]
    public float startDelay = 0.5f;

    // --- Private Variables ---
    private RectTransform _rectTransform;
    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private float _startTime;
    private bool _isReadyToMove = false;

    private void Awake()
    {
        // Cache the RectTransform component for better performance.
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // When the object is enabled (or at the start of the game), begin the setup.
        StartCoroutine(StartMovementDelay());
    }

    private IEnumerator StartMovementDelay()
    {
        // Instantly hide or reset state if needed
        _isReadyToMove = false;
        
        // Wait for the specified delay.
        yield return new WaitForSeconds(startDelay);

        // --- Setup positions after the delay ---
        // Record the anchored position when the movement is about to begin.
        _startPosition = _rectTransform.anchoredPosition;
        _endPosition = _startPosition + movementVector;
        
        // Record the precise start time for the animation loop.
        _startTime = Time.time;
        _isReadyToMove = true;
    }

    private void Update()
    {
        // Don't do anything until the start delay has passed.
        if (!_isReadyToMove)
        {
            return;
        }

        // --- The Core of the Smooth Movement ---

        // 1. Calculate the frequency of the oscillation based on the duration.
        // This ensures one half-cycle (A -> B) completes in exactly 'durationOneWay' seconds.
        float frequency = Mathf.PI / durationOneWay;

        // 2. Get the elapsed time since the movement started.
        float elapsedTime = Time.time - _startTime;

        // 3. Use the Cosine function to create a smooth 0-to-1-to-0 interpolation factor.
        // This creates the ease-in and ease-out effect automatically.
        float interpolationFactor = (1f - Mathf.Cos(elapsedTime * frequency)) / 2f;

        // 4. Use Vector2.Lerp to find the current position between the start and end points.
        Vector2 newPosition = Vector2.Lerp(_startPosition, _endPosition, interpolationFactor);

        // 5. Apply the new position to the RectTransform's anchoredPosition.
        _rectTransform.anchoredPosition = newPosition;
    }
}