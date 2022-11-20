using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class ShootingShuriken : MonoBehaviour
{
    [SerializeField] public SpriteRenderer shuriken;
    [SerializeField] public Transform shurikenTransform;
    [SerializeField] public Transform rotationPosition;
    [SerializeField] public MousePosTracker mouseTracker;
    [SerializeField] public float shurikenForce;

    private Vector3 mousePos;

    void Update()
    {
        mousePos = mouseTracker.GetMousePositionInWorld();
        Vector3 rotation = mousePos - rotationPosition.position;
        float zRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotationPosition.rotation = Quaternion.Euler(0, 0, zRotation);
    }

    public void SpawnShuriken(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        var shurikenObj = Instantiate(shuriken.gameObject, shurikenTransform.position, Quaternion.identity);
        ShootShuriken(shurikenObj);
    }

    private void ShootShuriken(GameObject shurikenObj)
    {
        var rb = shurikenObj.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
        
        Vector3 direction = mousePos - transform.position;
        Debug.DrawLine(shurikenTransform.position, mousePos, Color.red, 2.0f);
        rb.velocity = new Vector2(direction.x, direction.y).normalized * shurikenForce;
    }
}
