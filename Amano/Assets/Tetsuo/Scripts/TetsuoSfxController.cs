using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetsuoSfxController : MonoBehaviour
{
    [SerializeField] public AudioFactsScriptableObject TetsuoSoundFacts;
    private GenericDictionary<string, AudioClip> _tetsuoSoundMap;
    [SerializeField] private AudioSource _oneshotAudioSource;
    [SerializeField] private AudioSource _loopedAudioSource;
    
    void Start()
    {
        _tetsuoSoundMap = TetsuoSoundFacts.Facts;

        TetsuoController.PlaySoundEffectAction += PlayTetsuoSoundEffect;
        TetsuoHealthBar.TetsuoHurtOrDeathSoundAction += PlayTetsuoSoundEffect;
        TeleportAbility.TeleportSoundAction += PlayTetsuoSoundEffect;
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

    private void StopPlayingTetsuoSoundEffect(string soundEffectName)
    {
        if(_oneshotAudioSource.clip == _tetsuoSoundMap[soundEffectName])
            _oneshotAudioSource.Stop();
    }
}
