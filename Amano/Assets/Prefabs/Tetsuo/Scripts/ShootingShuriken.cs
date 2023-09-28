using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ShootingShuriken : MonoBehaviour
{
    [SerializeField] public SpriteRenderer shuriken;
    [SerializeField] private FactsScriptableObject AbilityFacts;
    
    [SerializeField] public Transform ShurikenThrowTransform;
    [SerializeField] public Transform rotationPosition;
    [SerializeField] public AimDirectionTracker aimTracker;
    [SerializeField] public float shurikenForce;
    [SerializeField] public float shurikenShootCooldown;

    [SerializeField] private LineRenderer _lineRenderer;
    
    [SerializeField] private float _linePoints = 25;
    /// <summary>
    /// Defines how many steps we need to take. Ex: every 0.01 seconds, a new point is defined
    /// </summary>
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

    public static Action<string> ShootShurikenSoundAction;

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
            Debug.Log($"Aim Rotation: {zRotation}");
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
        if (AbilityFacts.Facts["Shuriken"] == 0)
        {
            return;
        }
        
        if (context.started && canShoot)
        {
            ShootShurikenSoundAction?.Invoke("Shoot");
            var shurikenObj = ObjectPool.ObjectPoolInstance.GetFirstPooledObject();

            if (shurikenObj != null)
            {
                shurikenObj.SetActive(true);
                shurikenObj.GetComponent<Rigidbody2D>().simulated = true;
                shurikenObj.transform.position = ShurikenThrowTransform.position;
                shurikenObj.transform.rotation = Quaternion.identity;
                ShootShuriken(shurikenObj);
                canShoot = false;
                StartCoroutine(ShurikenCooldown());
            }
        }
    }
    
    public void CreateAimTrajectory(InputAction.CallbackContext context)
    {
        if (AbilityFacts.Facts["Shuriken"] == 0)
        {
            return;
        }
        
        if (context.performed && (aimTracker.CurrentInput == GameInputManager.InputType.KeyboardMouse && aimTracker.GetMousePositionInScreen() != Vector2.zero || aimTracker.GetRightStickDirection() != Vector2.zero))
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

        Vector2 direction = aimTracker.CurrentInput == GameInputManager.InputType.KeyboardMouse
            ? aimPos - transform.position
            : new Vector2(aimPos.x, aimPos.y);
        
        Vector2 startVelocity = direction.normalized * shurikenForce / 1f;

        int i = 0;
        _lineRenderer.SetPosition(i, startPosition);

        for (float time = 0; time < _linePoints; time += _timeBetweenPoints)
        {
            i++;
            var point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics2D.gravity.y / 2f * time * time);

            // Perform collision detection
            RaycastHit2D hit = Physics2D.Raycast(startPosition, point - startPosition, Vector2.Distance(point, startPosition), TrajectoryLayerMask);

            if (hit.collider != null)
            {
                // Collision detected
                // Do something with the collider or the hit information
                _lineRenderer.SetPosition(i - 1, hit.point);
                
                // Set the line renderer position count to ensure that it only renders up to the collision point
                _lineRenderer.positionCount = i;
                return;
            }

            _lineRenderer.SetPosition(i, point);
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
