using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] public GameObject Tetsuo;
    [SerializeField] public bool StartPlayingMusic;
    public static Action<Vector2> AdjustParallaxBackgroundAction;
    public static Action StartPlayingMusicAction;

    public void OnRoomEnter()
    {
        gameObject.SetActive(true);
        AdjustParallaxBackgroundAction?.Invoke(Tetsuo.transform.position);

        if (StartPlayingMusic)
            StartPlayingMusicAction?.Invoke();
    }
    
    public void OnRoomExit()
    {
        GameManager.Instance.ReturnAllShuriken();
        StartCoroutine(DisableRoom());
    }

    private IEnumerator DisableRoom()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}
