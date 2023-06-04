using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRolloff : MonoBehaviour
{
    [SerializeField] public float MinDistance;
    [SerializeField] public float MaxDistance;
    private AudioSource _audioSource;
    private Transform _tetsuoPosition;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _tetsuoPosition = GameObject.Find("Tetsuo").transform;
    }

    void Update()
    {
        RolloffVolumeBasedOnPlayerPosition();
    }

    private void RolloffVolumeBasedOnPlayerPosition()
    {
        // Get the position of the listener (e.g., the player or camera)
        var listenerPosition = _tetsuoPosition.transform.position;

        // Get the position of the audio source
        var audioSourcePosition = transform.position;

        // Calculate the distance between the listener and the audio source in 2D
        float distance = Vector2.Distance(listenerPosition, audioSourcePosition);
        
        // Calculate the volume based on the distance
        float volume = 1f - Mathf.Clamp01((distance - MinDistance) / (MaxDistance - MinDistance));

        // Set the volume of the audio source
        _audioSource.volume = volume;
    }
}
