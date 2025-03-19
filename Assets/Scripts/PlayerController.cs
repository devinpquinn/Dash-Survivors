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

    // Helper method to sync the shadow sprite with the main reticle sprite and offset it
    private void SyncShadowSprite(GameObject reticleInstance, Sprite sprite)
    {
        // Update the shadow sprite
        SpriteRenderer shadowSpriteRenderer = reticleInstance.transform.Find("Shadow").GetComponent<SpriteRenderer>();
        shadowSpriteRenderer.sprite = sprite;

        // Calculate the offset direction (opposite of the direction from the player to the reticle)
        Vector3 offsetDirection = (reticleInstance.transform.position - transform.position).normalized;

        // Apply the offset to the shadow's position
        float shadowOffsetDistance = 0.05f; // Adjust this value to control the shadow's distance
        shadowSpriteRenderer.transform.position = reticleInstance.transform.position + offsetDirection * -shadowOffsetDistance;
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
        
        StartCoroutine(DealDamageAlongDash(startPosition, target)); // Add this line

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
    
    IEnumerator DealDamageAlongDash(Vector3 startPos, Vector3 endPos)
    {
        // Calculate the player's width along the dash path
        float playerWidth = 0.5f; // Adjust this value to control the player's width

        // Calculate the dash direction and distance
        Vector3 dashDirection = (endPos - startPos).normalized;
        float dashDistance = Vector3.Distance(startPos, endPos);

        // Calculate the center and size of the dash path for the OverlapBox
        Vector3 dashCenter = (startPos + endPos) / 2;
        Vector2 dashSize = new Vector2(dashDistance, playerWidth);

        // Detect all enemies within the dash path using OverlapBox
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(dashCenter, dashSize, Vector2.SignedAngle(Vector2.right, dashDirection));
        List<Enemy> enemiesHit = new List<Enemy>();

        foreach (Collider2D collider in hitColliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesHit.Add(enemy);
            }
        }

        // Sort enemies by their distance from the start position
        enemiesHit.Sort((enemy1, enemy2) =>
        {
            float distance1 = Vector3.Distance(startPos, enemy1.transform.position);
            float distance2 = Vector3.Distance(startPos, enemy2.transform.position);
            return distance1.CompareTo(distance2);
        });

        // Start a separate coroutine for each enemy to apply damage after the calculated delay
        foreach (Enemy enemy in enemiesHit)
        {
            float distanceFromStart = Vector3.Distance(startPos, enemy.transform.position);
            float delay = Mathf.Lerp(0f, 0.1f, distanceFromStart / dashDistance);

            // Start a coroutine for this enemy
            StartCoroutine(ApplyDamageAfterDelay(enemy, delay, dashCenter, dashDistance));
        }

        yield return null;
    }

    private IEnumerator ApplyDamageAfterDelay(Enemy enemy, float delay, Vector3 dashCenter, float dashDistance)
    {
        // Wait for the calculated delay
        yield return new WaitForSeconds(delay);

        // Calculate damage based on distance to the midpoint
        float distanceToMidpoint = Vector3.Distance(enemy.transform.position, dashCenter);
        float maxDistanceToMidpoint = dashDistance / 2;
        float damage = Mathf.Lerp(10f, 0f, distanceToMidpoint / maxDistanceToMidpoint);
        damage += 1; //adjust damage to make 10 more likely and 0 impossible

        // Apply damage
        enemy.Hit((int)damage);
    }
}
