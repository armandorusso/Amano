using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public void OnRoomExit()
    {
        GameManager.Instance.ReturnAllShuriken();
    }
}
