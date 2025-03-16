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
    private GameObject reticleInstance;
    private SpriteRenderer reticleSpriteRenderer;

    void Start()
    {
        // Hide the mouse cursor
        Cursor.visible = false;

        // Instantiate the reticle and deactivate it initially
        reticleInstance = Instantiate(reticlePrefab);
        reticleInstance.SetActive(false);
        reticleSpriteRenderer = reticleInstance.GetComponent<SpriteRenderer>();
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
        }

        // Update reticle position and visibility
        UpdateReticle();
    }

    private void UpdateReticle()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Keep the z position the same

        float distance = Vector3.Distance(transform.position, mousePosition);
        if (distance >= minDistance && distance <= maxDistance)
        {
            reticleInstance.transform.position = mousePosition;
            reticleSpriteRenderer.sprite = validRangeSprite;
            reticleInstance.SetActive(true);
        }
        else
        {
            reticleInstance.transform.position = mousePosition;
            reticleSpriteRenderer.sprite = invalidRangeSprite;
            reticleInstance.SetActive(true);
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
