using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioSource audioSource;   
    public Transform transform;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on AudioManager GameObject.");
            return;
        }
        AudioTool.Initialize(audioSource,transform);
    }

}
