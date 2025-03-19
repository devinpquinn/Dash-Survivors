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

    public GameObject damagePopupPrefab; // Assign the DamagePopup prefab in the Inspector

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Get the Animator component once
        animator = GetComponent<Animator>();
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
                }
                else if (damage == 10)
                {
                    textMesh.color = new Color(1f, 0.84f, 0f); // Gold (RGB: 255, 215, 0)
                }
            }
        }
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }
}
