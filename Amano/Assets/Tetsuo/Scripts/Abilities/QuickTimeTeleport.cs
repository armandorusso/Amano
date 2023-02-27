using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickTimeTeleport : MonoBehaviour
{
    private bool isTimeSlowed;
    
    public class EnemyDamagedEventArgs : EventArgs
    {
        public float damage { get; set; }
        public GameObject enemy { get; set; }
        public LayerMask enemyLayer { get; set; }
    }

    public EnemyDamagedEventArgs enemyDamagedEventArgs;
    public static event EventHandler<EnemyDamagedEventArgs> EnemyDamagedEvent;
    
    public class ZoomInEventArgs : EventArgs
    {
        public float zoomInAmount;
    }

    public ZoomInEventArgs zoomInEventArgs;
    public static event EventHandler<ZoomInEventArgs> ZoomInEvent;
    
    void Start()
    {
        TeleportAbility.QuickTimeTeleportEvent += OnQuickTimeTeleportInvokeEvent;

        zoomInEventArgs = new ZoomInEventArgs
        {
            zoomInAmount = 3.4f
        };
    }

    private void OnQuickTimeTeleportInvokeEvent(object sender, TeleportAbility.QuickTimeTeleportEventArgs e)
    {
        var teleportedGameObject = e.objectBeingTeleported;
        var quickTimeComponent = teleportedGameObject.GetComponent<QuickTimeComponent>();
        quickTimeComponent.enabled = false;

        teleportedGameObject.transform.parent = null;
        teleportedGameObject.layer = LayerMask.NameToLayer("Item");
        e.enemy.layer = LayerMask.NameToLayer("Enemy");
        
        enemyDamagedEventArgs = new EnemyDamagedEventArgs
        {
            damage = 100f,
            enemyLayer = LayerMask.NameToLayer("Enemy"),
            enemy = e.enemy
        };

        StartCoroutine(SlowDownTime());
    }

    private IEnumerator SlowDownTime()
    {
        // Invoke Camera Zoom
        ZoomInEvent.Invoke(this, zoomInEventArgs);
        Time.timeScale = 0.25f;
        isTimeSlowed = true;
        yield return new WaitForSeconds(2f);
        Time.timeScale = 1f;
        isTimeSlowed = false;
        zoomInEventArgs.zoomInAmount = 5.4f;
        ZoomInEvent?.Invoke(this, zoomInEventArgs);
        zoomInEventArgs.zoomInAmount = 3.4f;
    }

    public void QuickTimeInput(InputAction.CallbackContext context)
    {
        if (isTimeSlowed && context.performed)
        {
            EnemyDamagedEvent?.Invoke(this, enemyDamagedEventArgs);
            StopCoroutine(SlowDownTime());
            Time.timeScale = 1f;
            isTimeSlowed = false;
            zoomInEventArgs.zoomInAmount = 5.4f;
            ZoomInEvent?.Invoke(this, zoomInEventArgs);
            zoomInEventArgs.zoomInAmount = 3.4f;
        }
    }

    private void OnDestroy()
    {
        TeleportAbility.QuickTimeTeleportEvent -= OnQuickTimeTeleportInvokeEvent;
    }
}
