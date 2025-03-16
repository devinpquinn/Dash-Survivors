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

    private Vector3 targetPosition;
    private bool isMoving = false;
    private GameObject validRangeReticleInstance;
    private GameObject invalidRangeReticleInstance;
    private SpriteRenderer validRangeReticleSpriteRenderer;
    private SpriteRenderer invalidRangeReticleSpriteRenderer;

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
            }
            else
            {
                // Calculate the clamped position for the valid range reticle
                Vector3 direction = (mousePosition - transform.position).normalized;
                targetPosition = transform.position + direction * Mathf.Clamp(distance, minDistance, maxDistance);
            }
            StartCoroutine(MoveToPosition(targetPosition));
        }

        // Update reticle positions and visibility
        UpdateReticles();
    }

    private void UpdateReticles()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Keep the z position the same

        float distance = Vector3.Distance(transform.position, mousePosition);
        if (distance >= minDistance && distance <= maxDistance)
        {
            validRangeReticleInstance.transform.position = mousePosition;
            validRangeReticleSpriteRenderer.sprite = validRangeSprite;
            validRangeReticleInstance.SetActive(true);

            invalidRangeReticleInstance.SetActive(false);
        }
        else
        {
            // Calculate the clamped position for the valid range reticle
            Vector3 direction = (mousePosition - transform.position).normalized;
            Vector3 clampedPosition = transform.position + direction * Mathf.Clamp(distance, minDistance, maxDistance);

            validRangeReticleInstance.transform.position = clampedPosition;
            validRangeReticleSpriteRenderer.sprite = validRangeSprite;
            validRangeReticleInstance.SetActive(true);

            invalidRangeReticleInstance.transform.position = mousePosition;
            invalidRangeReticleSpriteRenderer.sprite = invalidRangeSprite;
            invalidRangeReticleInstance.SetActive(true);
        }
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
