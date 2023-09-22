using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class TeleportAbility : MonoBehaviour
{
    [SerializeField] public float TeleportPopOutForce;
    private Queue<ShurikenProjectile> _normalShurikens;
    private Queue<ShurikenProjectile> _teleportShurikens;
    private Queue<Transform> _teleportableObjects;
    private bool canTeleport;
    private Rigidbody2D _rb;
    
    public class VanishingFxEventArgs : EventArgs
    {
        public GameObject objectBeingTeleported1 { get; set; }
        public GameObject objectBeingTeleported2 { get; set; }
    }
    public static event EventHandler<VanishingFxEventArgs> VanishingEvent;
    private VanishingFxEventArgs vanishingEventArgs;

    public static Action<float> TeleportPopOutAction;
    public static Action<string> TeleportSoundAction;
    public static Action<ShieldRotation> RemoveShieldComponentsAction;
    public static Action ShurikenConsumedAction;
    public static Action RemoveInteractButtonAction;

    void Awake()
    {
        ShurikenProjectile.ShurikenHitEvent += OnShurikenHitEvent;
        ShurikenProjectile.ShurikenAttachedEvent += OnShurikenAttachedEvent;

        vanishingEventArgs = new VanishingFxEventArgs
        {
            objectBeingTeleported1 = null,
            objectBeingTeleported2 = null
        };
    }

    private void Start()
    {
        _normalShurikens = new Queue<ShurikenProjectile>(10);
        _teleportShurikens = new Queue<ShurikenProjectile>(10);
        _teleportableObjects = new Queue<Transform>(10);
        canTeleport = false;
        _rb = GetComponent<Rigidbody2D>();

        GameManager.EnemyDeathEvent += OnEnemyDefeated;
    }

    private void OnShurikenHitEvent(object sender, ShurikenProjectile.ShurikenHitEventArgs e)
    {
        Debug.Log("Event invoked");

        if (sender is ShurikenProjectile shurikenProjectile)
        {
            _normalShurikens.Enqueue(shurikenProjectile);
            
            if (_normalShurikens.Count > 5)
            {
                var shurikenToRemove = _normalShurikens.Dequeue();
                
                ObjectPool.ObjectPoolInstance.ReturnPooledObject(shurikenToRemove.gameObject);
            }
        }
    }

    private void OnShurikenAttachedEvent(object sender, ShurikenProjectile.ShurikenAttachedEventArgs e)
    {
        canTeleport = e.objectCanTeleport;

        if (sender is ShurikenProjectile shurikenProjectile)
        {
            _teleportShurikens.Enqueue(shurikenProjectile);
            _teleportableObjects.Enqueue(e.teleportableObject);
            
            if (_teleportShurikens.Count > 5)
            {
                var shuriken = _teleportShurikens.Dequeue();
                _teleportableObjects.Dequeue();
                
                ObjectPool.ObjectPoolInstance.ReturnPooledObject(shuriken.gameObject);
            }
        }
    }

    private void OnEnemyDefeated(object sender, GameManager.EnemyDeathEventArgs e)
    {
        for (int i = 0; i < e.numberOfShuriken; i++)
        {
            _teleportShurikens.Dequeue();
            _teleportableObjects.Dequeue();
        }
    }

    public void TeleportToObject(InputAction.CallbackContext context)
    {
        if (context.performed && canTeleport && _teleportShurikens.Count > 0)
        {
            var shuriken = _teleportShurikens.Peek();
            Debug.Log("Swapping positions");
            _teleportShurikens.Dequeue();
            var objectToTeleport = _teleportableObjects.Dequeue();
            ShurikenConsumedAction?.Invoke();
            Assert.IsNotNull(shuriken);
            vanishingEventArgs.objectBeingTeleported1 = gameObject;
            vanishingEventArgs.objectBeingTeleported2 = objectToTeleport.gameObject;
            VanishingEvent.Invoke(this, vanishingEventArgs);
            TeleportSoundAction?.Invoke("Vanish");
            var playerPosition = gameObject.transform.position;
            gameObject.transform.position = objectToTeleport.position;
            // objectToTeleport.transform.position = playerPosition;

            ObjectPool.ObjectPoolInstance.ReturnPooledObject(shuriken.gameObject);

            if (objectToTeleport.TryGetComponent(out ShieldRotation shield))
            {
                RemoveShieldComponentsAction?.Invoke(shield);
            }

            TeleportPopOutAction?.Invoke(TeleportPopOutForce);

            if (_teleportShurikens.Count < 1)
            {
                canTeleport = false;
                RemoveInteractButtonAction?.Invoke();
            }
        }
    }

    public void ReturnAllShuriken()
    {
        while (_teleportableObjects.Count != 0)
        {
            _teleportableObjects.Dequeue();
        }

        while (_normalShurikens.Count != 0)
        {
            _normalShurikens.Dequeue();
        }
        
        while (_teleportShurikens.Count != 0)
        {
            _teleportShurikens.Dequeue();
        }
        
        // Return all the shuriken flying in the air as well
        ObjectPool.ObjectPoolInstance.ReturnAllPooledObjects();
    }

    private void OnDestroy()
    {
        ShurikenProjectile.ShurikenHitEvent -= OnShurikenHitEvent;
        ShurikenProjectile.ShurikenAttachedEvent -= OnShurikenAttachedEvent;
        GameManager.EnemyDeathEvent -= OnEnemyDefeated;
    }
}
