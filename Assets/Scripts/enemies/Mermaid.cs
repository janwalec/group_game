using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mermaid : EnemyController
{
    private bool stealth;
    private float stealthDuration = 2f;  // 2 seconds in stealth
    private float nonStealthDuration = 3f; // 3 seconds out of stealth
    private float fadeAmount = 0.3f;
    private SpriteRenderer spriteRenderer;
    private bool isStealthActive;

    private void Start()
    {

        health = 10;
        speed = 1.5f;
        Prepare();
        changeText(health.ToString());


        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing from Mermaid!");
        }

        // Start the stealth cycle coroutine
        StartCoroutine(StealthCycle());
    }

    public override void Move()
    {

        base.Move();
    }

    private IEnumerator StealthCycle()
    {
        while (true)
        {
            isStealthActive = true;
            yield return StartCoroutine(FadeOut());
            yield return new WaitForSeconds(stealthDuration);

            yield return StartCoroutine(FadeIn());
            isStealthActive = false;
            yield return new WaitForSeconds(nonStealthDuration);

        }
    }

    private IEnumerator FadeOut()
    {
        for (float t = 0; t < 1; t += Time.deltaTime / stealthDuration)
        {
            Color c = spriteRenderer.color;
            c.a = Mathf.Lerp(1f, fadeAmount, t);
            spriteRenderer.color = c;
            yield return null;
        }

    }

    private IEnumerator FadeIn()
    {
        for (float t = 0; t < 1; t += Time.deltaTime / stealthDuration)
        {
            Color c = spriteRenderer.color;
            c.a = Mathf.Lerp(fadeAmount, 1f, t);
            spriteRenderer.color = c;
            yield return null;
        }
    }


    public void MakeSpriteDisappear()
    {
        // Trigger the coroutine to make the sprite disappear for the stealth duration
        StartCoroutine(DisappearForSeconds());
    }

    private IEnumerator DisappearForSeconds()
    {
        //Debug.Log("Mermaid is in stealth mode.");
        isStealthActive = true;
        for (float t = 0; t < 1; t += Time.deltaTime / (stealthDuration / 2))
        {
            Color c = spriteRenderer.color;
            c.a = Mathf.Lerp(1f, fadeAmount, t);
            spriteRenderer.color = c;
            yield return null;
        }

        // Wait for the specified stealth duration

        yield return new WaitForSeconds(stealthDuration / 2);

        for (float t = 0; t < 1; t += Time.deltaTime / (stealthDuration / 2))
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(fadeAmount, 1f, t);
            spriteRenderer.color = color;
            yield return null;
        }

        isStealthActive = false;
        //Debug.Log("Mermaid is visible again.");
    }


    public override void TakeDamage(int dmg)
    {
        Debug.Log("In take damage");
        if (!isStealthActive)
        {
            Debug.Log("Mermaid Taking damage");
            base.TakeDamage(dmg);
            
        }
        else
        {
            Debug.Log("In stealth mode, can't receive damage");
        }

    }
   /* public override void Die()
    {
        Debug.Log("Mermaid has died.");
        base.Die();
    }*/
}
