using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectOnStart : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(true);
    }
}
