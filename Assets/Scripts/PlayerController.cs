using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float minDistance = 1f;
    public float maxDistance = 10f;
    public GameObject reticlePrefab;
    public Sprite validRangeSprite;
    public Sprite invalidRangeSprite;
    public Sprite midpointSprite; // Add this line

    private Vector3 targetPosition;
    private bool isMoving = false;
    private GameObject validRangeReticleInstance;
    private GameObject invalidRangeReticleInstance;
    private GameObject midpointReticleInstance; // Add this line
    private SpriteRenderer validRangeReticleSpriteRenderer;
    private SpriteRenderer invalidRangeReticleSpriteRenderer;
    private SpriteRenderer midpointReticleSpriteRenderer; // Add this line

    void Start()
    {
        // Hide the mouse cursor
        Cursor.visible = false;

        // Instantiate the reticles and deactivate them initially
        validRangeReticleInstance = Instantiate(reticlePrefab);
        validRangeReticleInstance.SetActive(false);
        validRangeReticleSpriteRenderer = validRangeReticleInstance.GetComponent<SpriteRenderer>();

        invalidRangeReticleInstance = Instantiate(reticlePrefab);
        invalidRangeReticleInstance.SetActive(false);
        invalidRangeReticleSpriteRenderer = invalidRangeReticleInstance.GetComponent<SpriteRenderer>();

        // Instantiate the midpoint reticle and deactivate it initially
        midpointReticleInstance = Instantiate(reticlePrefab); // Add this block
        midpointReticleInstance.SetActive(false);
        midpointReticleSpriteRenderer = midpointReticleInstance.GetComponent<SpriteRenderer>();
        midpointReticleSpriteRenderer.sprite = midpointSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z; // Keep the z position the same

            float distance = Vector3.Distance(transform.position, mousePosition);
            if (distance >= minDistance && distance <= maxDistance)
            {
                targetPosition = mousePosition;
                StartCoroutine(MoveToPosition(targetPosition));
            }
            else if (distance > maxDistance)
            {
                // Calculate the clamped position for the valid range reticle
                Vector3 direction = (mousePosition - transform.position).normalized;
                targetPosition = transform.position + direction * maxDistance;
                StartCoroutine(MoveToPosition(targetPosition));
            }
        }

        // Update reticle positions and visibility
        UpdateReticles();
    }

    private void UpdateReticles()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Keep the z position the same
        
        // If mouse is at the exact position of the player, hide all reticles
        if (mousePosition == transform.position)
        {
            validRangeReticleInstance.SetActive(false);
            midpointReticleInstance.SetActive(false);
            invalidRangeReticleInstance.SetActive(false);
            return;
        }
        
        float distance = Vector3.Distance(transform.position, mousePosition);
        if (distance < minDistance)
        {
            validRangeReticleInstance.SetActive(false);
            midpointReticleInstance.SetActive(false);

            invalidRangeReticleInstance.transform.position = mousePosition;
            invalidRangeReticleSpriteRenderer.sprite = invalidRangeSprite;
            SyncShadowSprite(invalidRangeReticleInstance, invalidRangeSprite); // Sync shadow sprite
            invalidRangeReticleInstance.SetActive(true);
            return;
        }
        
        if (distance >= minDistance && distance <= maxDistance)
        {
            validRangeReticleInstance.transform.position = mousePosition;
            validRangeReticleSpriteRenderer.sprite = validRangeSprite;
            SyncShadowSprite(validRangeReticleInstance, validRangeSprite); // Sync shadow sprite
            validRangeReticleInstance.SetActive(true);

            invalidRangeReticleInstance.SetActive(false);

            // Update midpoint reticle position
            Vector3 midpointPosition = (transform.position + mousePosition) / 2;
            midpointReticleInstance.transform.position = midpointPosition;
            midpointReticleSpriteRenderer.sprite = midpointSprite;
            SyncShadowSprite(midpointReticleInstance, midpointSprite); // Sync shadow sprite
            midpointReticleInstance.SetActive(true);
        }
        else
        {
            // Calculate the clamped position for the valid range reticle
            Vector3 direction = (mousePosition - transform.position).normalized;
            Vector3 clampedPosition = transform.position + direction * Mathf.Clamp(distance, minDistance, maxDistance);

            validRangeReticleInstance.transform.position = clampedPosition;
            validRangeReticleSpriteRenderer.sprite = validRangeSprite;
            SyncShadowSprite(validRangeReticleInstance, validRangeSprite); // Sync shadow sprite
            validRangeReticleInstance.SetActive(true);

            invalidRangeReticleInstance.transform.position = mousePosition;
            invalidRangeReticleSpriteRenderer.sprite = invalidRangeSprite;
            SyncShadowSprite(invalidRangeReticleInstance, invalidRangeSprite); // Sync shadow sprite
            invalidRangeReticleInstance.SetActive(true);

            // Update midpoint reticle position
            Vector3 midpointPosition = (transform.position + clampedPosition) / 2;
            midpointReticleInstance.transform.position = midpointPosition;
            midpointReticleSpriteRenderer.sprite = midpointSprite;
            SyncShadowSprite(midpointReticleInstance, midpointSprite); // Sync shadow sprite
            midpointReticleInstance.SetActive(true);
        }
    }

    // Helper method to sync the shadow sprite with the main reticle sprite
    private void SyncShadowSprite(GameObject reticleInstance, Sprite sprite)
    {
        SpriteRenderer shadowSpriteRenderer = reticleInstance.transform.Find("Shadow").GetComponent<SpriteRenderer>();
        shadowSpriteRenderer.sprite = sprite;
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        float duration = 0.1f;

        // First phase: ease into acceleration
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, target, elapsedTime / duration * 0.1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move to 90% of the way instantly
        transform.position = Vector3.Lerp(startPosition, target, 0.9f);

        // Second phase: decelerate to the target
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(transform.position, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the player reaches the exact target position
        transform.position = target;
        isMoving = false;
    }
}
