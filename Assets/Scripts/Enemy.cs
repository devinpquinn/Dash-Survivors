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
                if (damage >= 1 && damage <= 4)
                {
                    textMesh.color = Color.grey;
                }
                else if (damage >= 5 && damage <= 9)
                {
                    textMesh.color = Color.white;
                }
                else if (damage == 10)
                {
                    textMesh.color = Color.yellow;
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
