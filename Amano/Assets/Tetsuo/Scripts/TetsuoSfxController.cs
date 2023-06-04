using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetsuoSfxController : MonoBehaviour
{
    [SerializeField] public AudioFactsScriptableObject TetsuoSoundFacts;
    [SerializeField] private AudioSource _oneshotAudioSource;
    [SerializeField] private AudioSource _vanishAudioSource;
    [SerializeField] private AudioSource _loopedAudioSource;
    [SerializeField] private AudioSource _extraAudioSource;
    
    private GenericDictionary<string, AudioClip> _tetsuoSoundMap;

    void Start()
    {
        _tetsuoSoundMap = TetsuoSoundFacts.Facts;

        TetsuoHealthBar.TetsuoHurtOrDeathSoundAction += PlayTetsuoSoundEffect;
        TeleportAbility.TeleportSoundAction += PlayVanishSound;
        TetsuoController.PlaySoundEffectAction += PlayTetsuoSoundEffect;
        TetsuoController.PlayExtraSoundEffectAction += PlayMomentumSound;
        TetsuoController.StopSoundEffectAction += StopPlayingTetsuoSoundEffect;
        TetsuoController.PlayRunSoundEffectAction += PlayTetsuoRunSound;
    }

    private void PlayTetsuoSoundEffect(string soundEffectName)
    {
        if (_oneshotAudioSource.clip != _tetsuoSoundMap[soundEffectName])
        {
            _oneshotAudioSource.Stop();
        }
        
        if (!_oneshotAudioSource.isPlaying)
        {
            _oneshotAudioSource.PlayOneShot(_tetsuoSoundMap[soundEffectName]);
        }
    }

    private void PlayTetsuoRunSound(bool isRunning)
    {
        if (isRunning)
        {
            _loopedAudioSource.clip = _tetsuoSoundMap["Running"];
            
            if (!_loopedAudioSource.isPlaying)
            {
                _loopedAudioSource.Play();
            }
        }
        else
        {
            _loopedAudioSource.Stop();
        }
    }

    private void PlayMomentumSound(string soundEffectName)
    {
        if (_extraAudioSource.clip != _tetsuoSoundMap[soundEffectName])
        {
            _extraAudioSource.Stop();
        }
        
        if (!_extraAudioSource.isPlaying)
        {
            _extraAudioSource.PlayOneShot(_tetsuoSoundMap[soundEffectName]);
        }
    }
    
    private void PlayVanishSound(string soundEffectName)
    {
        if (_vanishAudioSource.clip != _tetsuoSoundMap[soundEffectName])
        {
            _vanishAudioSource.Stop();
        }
        
        if (!_extraAudioSource.isPlaying)
        {
            _vanishAudioSource.PlayOneShot(_tetsuoSoundMap[soundEffectName]);
        }
    }


    private void StopPlayingTetsuoSoundEffect(string soundEffectName)
    {
        if(_oneshotAudioSource.clip == _tetsuoSoundMap[soundEffectName])
            _oneshotAudioSource.Stop();
    }
}
