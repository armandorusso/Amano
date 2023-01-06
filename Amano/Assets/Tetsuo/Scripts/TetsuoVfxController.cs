using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetsuoVfxController : MonoBehaviour
{
    [SerializeField] private ParticleSystem groundDustFx;
    [SerializeField] private ParticleSystem wallDustFx;
    [SerializeField] private ParticleSystem leavesFx;

    private bool isWallDustPlaying;
    private bool isLeavesFlyingPlaying;

    private void Start()
    {
        TetsuoController.wallSlidingEvent += OnWallSlidingEvent;
        TetsuoController.jumpOrLandEvent += OnPlayerJumpOrLanding;
        TetsuoController.dashAttackEvent += OnDashAttackEvent;
    }

    private void OnWallSlidingEvent(object sender, TetsuoController.WallSlidingFxEventArgs e)
    {
        if (!e.isSliding)
        {
            if (isWallDustPlaying)
            {
                Debug.Log("Stop WallDust Playing");
                wallDustFx.Stop();
                isWallDustPlaying = false;
            }
        }

        if (e.isSliding)
        {
            if (isWallDustPlaying)
                return;
            
            Debug.Log("Dust wall playing");
            var transformLocalScale = wallDustFx.gameObject.transform.localScale;
            if (e.isFacingRight)
            {
                transformLocalScale.x = -1f;
            }
            else
            {
                transformLocalScale.x = 1f;
            }
            wallDustFx.Play();
            isWallDustPlaying = true;
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

    public void OnDashAttackEvent(object sender, TetsuoController.DashAttackFxEventArgs e)
    {
        if (e.isDashing)
        {
            leavesFx.Play();
        }
        else
        {
            leavesFx.Stop();
        }
    }
}
