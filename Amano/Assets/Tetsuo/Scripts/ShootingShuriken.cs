using System;
using System.Collections;
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
    [SerializeField] public float shurikenShootCooldown;
    
    public static event EventHandler<ShurikenSpawnedEventArgs> ShurikenSpawnedEvent;

    public class ShurikenSpawnedEventArgs : EventArgs
    {
        public GameObject shuriken { get; set; }
    }

    private ShurikenSpawnedEventArgs args;

    private bool canShoot;

    private Vector3 mousePos;

    private void Start()
    {
        canShoot = true;
    }

    void Update()
    {
        mousePos = mouseTracker.GetMousePositionInWorld();
        Vector3 rotation = mousePos - rotationPosition.position;
        float zRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotationPosition.rotation = Quaternion.Euler(0, 0, zRotation);

        if (transform.localScale.x != rotationPosition.localScale.x)
        {
            rotationPosition.localScale = transform.localScale;
        }
    }

    public void SpawnShuriken(InputAction.CallbackContext context)
    {
        if (context.started && canShoot)
        {
            var shurikenObj = ObjectPool.ObjectPoolInstance.GetFirstPooledObject(); // Instantiate(shuriken.gameObject, shurikenTransform.position, Quaternion.identity);
            shurikenObj.SetActive(true);
            shurikenObj.GetComponent<Rigidbody2D>().simulated = true;
            shurikenObj.transform.position = shurikenTransform.position;
            shurikenObj.transform.rotation = Quaternion.identity;
            ShootShuriken(shurikenObj);
            canShoot = false;
            StartCoroutine(ShurikenCooldown());
        }
    }

    private IEnumerator ShurikenCooldown()
    {
        yield return new WaitForSeconds(shurikenShootCooldown);
        canShoot = true;
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
