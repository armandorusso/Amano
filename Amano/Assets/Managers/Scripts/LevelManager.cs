using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public Animator _transitionSpriteAnimator;
    [SerializeField] public UnityEvent DisablePlayerMovementEvent;

    public static LevelManager Instance;
    private bool hasEnteredLevelTransition;

    public static Action ZoomInCameraAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNewLevel(string levelName)
    {
        if (!hasEnteredLevelTransition)
        {
            hasEnteredLevelTransition = true;
            DisablePlayerMovementEvent?.Invoke();
            var scene = SceneManager.LoadSceneAsync(levelName);
            scene.allowSceneActivation =
                false; // Don't immediately change the scene, let the screen transition play out first
            ZoomInCameraAction?.Invoke();
            StartCoroutine(StartScreenTransition(scene));
        }
    }

    private IEnumerator StartScreenTransition(AsyncOperation sceneTask)
    {
        _transitionSpriteAnimator.SetBool("canDarkenScene", true);
        
        // Wait an extra 2 seconds for effect
        yield return new WaitForSeconds(4f);
        
        _transitionSpriteAnimator.SetBool("canDarkenScene", false);

        hasEnteredLevelTransition = false;
        sceneTask.allowSceneActivation = true;
    }
}
