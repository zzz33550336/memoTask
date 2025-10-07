using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonTools
{
    public static float maxFallSpeed = -8f;

    public static float xBoundary = 9f;
    public static float yBoundary = 6f;
    public static void CheckScreenWrap(Transform transform, Rigidbody2D rb,bool isAlive)
    {
        if (!isAlive) return;

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

    public static IEnumerator FlashCoroutine(Renderer renderer, int flashTimes, float flashInterval, System.Action onFlashComplete)
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
    
    public static void addScore(int score)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.score += score;
            Debug.Log("Score increased by " + score + ". Total score: " + GameManager.Instance.score);
        }
        else
        {
            Debug.LogError("GameManager instance is null!");
        }
    }
}

public static class AudioTool
{
    
    public static AudioSource audioSource;    
    private static Vector3 position;
    public static void Initialize(AudioSource source,Transform transform)
    {
        audioSource = source;
        position = transform.position;
    }
    
    public static void PlaySound(string soundName)
    {
        AudioClip clip = Resources.Load<AudioClip>("audio/" + soundName);
        if (clip == null)
        {
            Debug.LogError("Sound not found: " + soundName);
            return;
        }
        
        AudioSource.PlayClipAtPoint(clip, position);
    }
    
}