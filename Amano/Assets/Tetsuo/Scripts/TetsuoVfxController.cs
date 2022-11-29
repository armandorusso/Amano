using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetsuoVfxController : MonoBehaviour
{
    [SerializeField] private ParticleSystem groundDustFx;
    [SerializeField] private ParticleSystem wallDustFx;

    private void Start()
    {
        TetsuoController.wallSlidingEvent += OnWallSlidingEvent;
        TetsuoController.jumpOrLandEvent += OnPlayerJumpOrLanding;
    }

    private void OnWallSlidingEvent(object sender, TetsuoController.WallSlidingFxEventArgs e)
    {
        if (e.isSliding)
        {
            wallDustFx.Play();
        }
    }

    public void OnPlayerJumpOrLanding(object sender, TetsuoController.GroundFxEventArgs e)
    {
        if (e.isDustActivated)
        {
            groundDustFx.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.3f, gameObject.transform.position.z);
            groundDustFx.Play();
        }
    }
}
