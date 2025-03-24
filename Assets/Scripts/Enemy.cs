using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private Animator animator; // Store the Animator component
    private Rigidbody2D rb; // Add a reference to Rigidbody2D

    public GameObject damagePopupPrefab; // Assign the DamagePopup prefab in the Inspector
    public int health = 10; // Add health property
    private Transform target; // Reference to the player
    public Color damagedColor = Color.red; // Add a damaged color property
    public Sprite deathSprite; // Add a death sprite property

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Get the Animator component once
        animator = GetComponent<Animator>();

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Move toward the player using Rigidbody2D
        if (target != null && rb != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction; // Set velocity to move toward the player
        }
    }

    public void SetTarget(Transform playerTransform)
    {
        target = playerTransform;
    }

    public void Hit(int damage)
    {
        if (spriteRenderer != null)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashWhite());
        }

        // Restart the hit animation
        if (animator != null)
        {
            animator.Play("EnemyHit", 0, 0f);
        }

        // Show the damage popup
        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            TextMeshPro textMesh = popup.GetComponentInChildren<TextMeshPro>();
            if (textMesh != null)
            {
                textMesh.text = damage.ToString();

                // Set the text color based on the damage value
                if (damage >= 1 && damage <= 9)
                {
                    // Interpolate between dark grey and white
                    Color darkGrey = new Color(0.2f, 0.2f, 0.2f); // Dark grey (RGB: 51, 51, 51)
                    Color white = Color.white; // White (RGB: 255, 255, 255)
                    float t = (damage - 1) / 8f; // Normalize damage to a range of 0 to 1
                    textMesh.color = Color.Lerp(darkGrey, white, t);

                    // Lerp font size between 5 and 8
                    textMesh.fontSize = Mathf.Lerp(5f, 7f, t);
                }
                else if (damage == 10)
                {
                    textMesh.color = new Color(1f, 0.84f, 0f); // Gold (RGB: 255, 215, 0)
                    textMesh.fontSize = 10f; // Set font size to 10 for max damage
                }
            }
        }

        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        // Stop movement before dying
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // Disable all colliders
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        // Flash white and then switch to the death sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f); // Short flash duration

            // Switch to the death sprite
            spriteRenderer.sprite = deathSprite;
        }

        yield return new WaitForSeconds(0.2f); // Wait before destroying the object
        Destroy(gameObject);
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.2f);

        // Update the sprite color based on health after flashing white
        if (spriteRenderer != null)
        {
            float healthPercentage = Mathf.Clamp01((float)health / 10f); // Normalize health to a range of 0 to 1
            spriteRenderer.color = Color.Lerp(damagedColor, originalColor, healthPercentage);
        }
    }
}
