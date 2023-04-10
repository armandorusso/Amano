using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ShootingShuriken : MonoBehaviour
{
    [SerializeField] public SpriteRenderer shuriken;
    [SerializeField] public Transform ShurikenThrowTransform;
    [SerializeField] public Transform rotationPosition;
    [SerializeField] public AimDirectionTracker aimTracker;
    [SerializeField] public float shurikenForce;
    [SerializeField] public float shurikenShootCooldown;

    [SerializeField] private LineRenderer _lineRenderer;
    
    [SerializeField] private float _linePoints = 25;
    [SerializeField] [Range(0.01f, 0.25f)] private float _timeBetweenPoints;
    [SerializeField] private LayerMask TrajectoryLayerMask;
    private bool _isHoldingAim;

    public static event EventHandler<ShurikenSpawnedEventArgs> ShurikenSpawnedEvent;

    public class ShurikenSpawnedEventArgs : EventArgs
    {
        public GameObject shuriken { get; set; }
    }

    private ShurikenSpawnedEventArgs args;

    private bool canShoot;

    private Vector3 aimPos;

    private void Start()
    {
        canShoot = true;
    }

    void Update()
    {
        if (aimTracker.CurrentInput == GameInputManager.InputType.KeyboardMouse)
        {
            aimPos = aimTracker.GetMousePositionInWorld();

            Vector3 rotation = aimPos - rotationPosition.position;
            float zRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            rotationPosition.rotation = Quaternion.Euler(0, 0, zRotation);
        }

        else
        {
            aimPos = aimTracker.GetRightStickDirection();
            float zRotation = Mathf.Atan2(aimPos.y, aimPos.x) * Mathf.Rad2Deg;
            rotationPosition.rotation = Quaternion.Euler(0, 0, zRotation);
        }
        if (transform.localScale.x != rotationPosition.localScale.x)
        {
            rotationPosition.localScale = transform.localScale;
        }

        if (_isHoldingAim)
            DrawAimTrajectory();
        else
            _lineRenderer.enabled = false;
    }

    public void SpawnShuriken(InputAction.CallbackContext context)
    {
        if (context.started && canShoot)
        {
            var shurikenObj = ObjectPool.ObjectPoolInstance.GetFirstPooledObject();
            shurikenObj.SetActive(true);
            shurikenObj.GetComponent<Rigidbody2D>().simulated = true;
            shurikenObj.transform.position = ShurikenThrowTransform.position;
            shurikenObj.transform.rotation = Quaternion.identity;
            ShootShuriken(shurikenObj);
            canShoot = false;
            StartCoroutine(ShurikenCooldown());
        }
    }
    
    public void CreateAimTrajectory(InputAction.CallbackContext context)
    {
        if (aimTracker.GetRightStickDirection() != Vector2.zero && context.performed)
        {
            _isHoldingAim = true;
        }
        else
        {
            _isHoldingAim = false;
        }
    }

    private void DrawAimTrajectory()
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenPoints) + 1;
        Vector2 startPosition = ShurikenThrowTransform.position;
        Vector2 startVelocity = (new Vector2(aimPos.x, aimPos.y).normalized * shurikenForce) / 1f;

        int i = 0;
        _lineRenderer.SetPosition(i, startPosition);

        for (float time = 0; time < _linePoints; time += _timeBetweenPoints)
        {
            i++;
            var point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics2D.gravity.y / 2f * time * time);

            _lineRenderer.SetPosition(i, point);
            
            // Make the line renderer stop when it hits a wall/collideable object
            /*Vector2 lastPosition = _lineRenderer.GetPosition(i - 1);
            var hit = Physics2D.Raycast(lastPosition, (point - lastPosition).normalized,
                (point - lastPosition).magnitude);

            if (hit.collider.gameObject.layer != TrajectoryLayerMask)
            {
                _lineRenderer.SetPosition(i, hit.point);
                _lineRenderer.positionCount = i + 1; // avoids the old points to be considered
                return;
            }*/
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

        if (aimTracker.CurrentInput == GameInputManager.InputType.KeyboardMouse)
        {
            Vector3 direction = aimPos - transform.position;
            rb.velocity = new Vector2(direction.x, direction.y).normalized * shurikenForce;
        }
        else
        {
            rb.velocity = new Vector2(aimPos.x, aimPos.y).normalized * shurikenForce;
        }
        Debug.DrawLine(ShurikenThrowTransform.position, aimPos, Color.red, 2.0f);
    }
}
