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
        public LayerMask enemyLayer { get; set; }
    }

    public EnemyDamagedEventArgs enemyDamagedEventArgs;
    public static event EventHandler<EnemyDamagedEventArgs> EnemyDamagedEvent;
    
    void Start()
    {
        TeleportAbility.QuickTimeTeleportEvent += OnQuickTimeTeleportInvokeEvent;
    }

    private void OnQuickTimeTeleportInvokeEvent(object sender, TeleportAbility.QuickTimeTeleportEventArgs e)
    {
        var teleportedGameObject = e.objectBeingTeleported;
        var rb = teleportedGameObject.GetComponent<Rigidbody2D>();
        rb.simulated = true;

        teleportedGameObject.transform.parent = null;
        teleportedGameObject.layer = LayerMask.NameToLayer("Item");
        
        enemyDamagedEventArgs = new EnemyDamagedEventArgs
        {
            damage = 100f,
            enemyLayer = LayerMask.NameToLayer("Enemy")
        };
        
        // Invoke Camera Zoom
        
        StartCoroutine(SlowDownTime());
    }

    private IEnumerator SlowDownTime()
    {
        Time.timeScale = 0.25f;
        isTimeSlowed = true;
        yield return new WaitForSeconds(2f);
        Time.timeScale = 1f;
        isTimeSlowed = false;
    }

    public void QuickTimeInput(InputAction.CallbackContext context)
    {
        if (isTimeSlowed && (context.started || context.performed))
        {
            EnemyDamagedEvent.Invoke(this, enemyDamagedEventArgs);
            StopCoroutine(SlowDownTime());
            Time.timeScale = 1f;
            isTimeSlowed = false;
        }
    }
}
