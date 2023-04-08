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
        TetsuoController.WallSlidingEffectAction += OnWallSlidingEvent;
        TetsuoController.JumpOrLandEffectAction += OnPlayerJumpOrLanding;
        TetsuoController.DashAttackLeafEffectAction += OnDashAttackEvent;
    }

    private void OnWallSlidingEvent(bool isSliding, bool isFacingRight)
    {
        if (!isSliding)
        {
            if (isWallDustPlaying)
            {
                Debug.Log("Stop WallDust Playing");
                wallDustFx.Stop();
                isWallDustPlaying = false;
            }
        }

        if (isSliding)
        {
            if (isWallDustPlaying)
                return;
            
            Debug.Log("Dust wall playing");
            var transformLocalScale = wallDustFx.gameObject.transform.localScale;
            if (isFacingRight)
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

    public void OnPlayerJumpOrLanding(bool isDustActivated)
    {
        if (isDustActivated)
        {
            groundDustFx.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.3f, gameObject.transform.position.z);
            groundDustFx.Play();
        }
    }

    public void OnDashAttackEvent(bool isDashing)
    {
        if (isDashing)
        {
            leavesFx.Play();
        }
        else
        {
            leavesFx.Stop();
        }
    }

    private void OnDestroy()
    {
        TetsuoController.WallSlidingEffectAction -= OnWallSlidingEvent;
        TetsuoController.JumpOrLandEffectAction -= OnPlayerJumpOrLanding;
        TetsuoController.DashAttackLeafEffectAction -= OnDashAttackEvent;
    }
}
