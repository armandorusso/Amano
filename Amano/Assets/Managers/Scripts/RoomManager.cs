using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] public GameObject Tetsuo;
    public static Action<Vector2> AdjustParallaxBackgroundAction;

    public void OnRoomEnter()
    {
        gameObject.SetActive(true);
        AdjustParallaxBackgroundAction?.Invoke(Tetsuo.transform.position);
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
