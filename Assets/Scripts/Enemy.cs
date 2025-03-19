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

    public GameObject damagePopupPrefab; // Assign the DamagePopup prefab in the Inspector

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
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

        // Show the damage popup
        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            TextMeshPro textMesh = popup.GetComponentInChildren<TextMeshPro>();
            if (textMesh != null)
            {
                textMesh.text = damage.ToString();
            }
        }
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
}
