using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetsuoHealthBar : MonoBehaviour
{
    // Implement Depencency Inversion properly here
    // The Health class doesnt need to be here. This class should implement the IHealth Interface and the decrease and increase health functions should invoke the events
    // Instead, have the Health class be attached to Tetsuo?
    // To be honest, not sure if we can really apply it here
    private IHealth TetsuoHealthPoints;
    public class HealthUIEventArgs : EventArgs
    {
        public float currentHealth { get; set; }
    }

    public HealthUIEventArgs healthUIEventArgs;
    public static event EventHandler<HealthUIEventArgs> healthUIEvent;

    void Start()
    {
        healthUIEventArgs = new HealthUIEventArgs
        {
            currentHealth = 100f
        };
        TetsuoHealthPoints = new Health(100f);
        healthUIEvent.Invoke(this, healthUIEventArgs);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Teleport"))
        {
            TetsuoHealthPoints.DecreaseHealth(20f);
            healthUIEventArgs.currentHealth = TetsuoHealthPoints.HitPoints;
            healthUIEvent.Invoke(this, healthUIEventArgs);
        }
    }
}
