using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        RoomManager.StartPlayingMusicAction += OnEnteredRoom;
    }

    private void OnEnteredRoom()
    {
        if(!_audioSource.isPlaying)
            _audioSource.PlayDelayed(0.8f);
    }
}
