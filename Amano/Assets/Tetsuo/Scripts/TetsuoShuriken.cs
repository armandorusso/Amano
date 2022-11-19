using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetsuoShuriken : MonoBehaviour
{
    [SerializeField] public SpriteRenderer shuriken;
    [SerializeField] public Transform shurikenPosition;

    // Update is called once per frame
    void Start()
    {
        
    }

    public void ShootShuriken(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        var shurikenObj = Instantiate(shuriken.gameObject, shurikenPosition.position, Quaternion.identity);
    }
}
