using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonTools
{
    public static float maxFallSpeed = -5f;

    public static float xBoundary = 9f;
    public static float yBoundary = 5f;
    public static void CheckScreenWrap(Transform transform, Rigidbody2D rb)
    {

        if (transform.position.x > xBoundary)
        {
            transform.position = new Vector3(-xBoundary, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -xBoundary)
        {
            transform.position = new Vector3(xBoundary, transform.position.y, transform.position.z);
        }
        if (transform.position.y > yBoundary)
        {
            transform.position = new Vector3(transform.position.x, -yBoundary, transform.position.z);
        }
        else if (transform.position.y < -yBoundary)
        {
            transform.position = new Vector3(transform.position.x, yBoundary, transform.position.z);
        }
        if (rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
    }
    
    public static void TakeDamageEffect(GameObject target, int flashTimes, float flashInterval, MonoBehaviour behaviour, System.Action onFlashComplete = null)
    {
        behaviour.StartCoroutine(FlashCoroutine(target.GetComponent<Renderer>(), flashTimes, flashInterval, onFlashComplete));
    }

    private static IEnumerator FlashCoroutine(Renderer renderer, int flashTimes, float flashInterval, System.Action onFlashComplete)
    {
        Color originColor = renderer.material.color;
        for (int i = 0; i < flashTimes; i++)
        {
            renderer.material.color = new Color(originColor.r, originColor.g, originColor.b, 0.3f);
            yield return new WaitForSeconds(flashInterval);
            renderer.material.color = originColor;
            yield return new WaitForSeconds(flashInterval);
        }
        
        onFlashComplete?.Invoke();
    }
}