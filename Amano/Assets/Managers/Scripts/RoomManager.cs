using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public void OnRoomEnter()
    {
        gameObject.SetActive(true);    
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
