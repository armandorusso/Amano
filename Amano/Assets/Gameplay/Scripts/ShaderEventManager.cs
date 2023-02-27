using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderEventManager : MonoBehaviour
{
    [SerializeField] public Material vanishingMaterial;
    private bool isShaderPlaying;

    private void Awake()
    {
        TeleportAbility.VanishingEvent += OnTeleportEvent;
        TetsuoController.groundedDashAttackEvent += OnDashEvent;
        isShaderPlaying = false;
    }

    private void OnTeleportEvent(object sender, TeleportAbility.VanishingFxEventArgs e)
    {
        var teleportSubject = e.objectBeingTeleported1;
        var teleportedObject = e.objectBeingTeleported2;

        StartCoroutine(PlayVanishingVFX(teleportSubject, teleportedObject));
    }

    private void OnDashEvent(object sender, TetsuoController.GroundedDashAttackFxEventArgs e)
    {
        StartCoroutine(PlaySpeedFx(e.Tetsuo));
    }

    private IEnumerator PlayVanishingVFX(GameObject subject, GameObject teleportedObject)
    {
        if (!isShaderPlaying)
        {
            isShaderPlaying = true;
            var spriteSubject = subject.GetComponent<SpriteRenderer>();
            var spriteObjectBeingTeleported = teleportedObject.GetComponent<SpriteRenderer>();

            var originalSubjectMat = spriteSubject.material;
            var originalObjectBeingTeleportedMat = spriteObjectBeingTeleported.material;

            spriteSubject.material = vanishingMaterial;
            spriteObjectBeingTeleported.material = vanishingMaterial;
            yield return new WaitForSeconds(0.2f);
            spriteSubject.material = originalSubjectMat;
            spriteObjectBeingTeleported.material = originalObjectBeingTeleportedMat;
        }

        isShaderPlaying = false;
    }

    private IEnumerator PlaySpeedFx(GameObject tetsuo)
    {
        var spriteSubject = tetsuo.GetComponent<SpriteRenderer>();
        var material = spriteSubject.material;
        spriteSubject.material = vanishingMaterial;
        yield return new WaitForSeconds(0.4f);
        spriteSubject.material = material;
    }

    private void OnDestroy()
    {
        TeleportAbility.VanishingEvent -= OnTeleportEvent;
        TetsuoController.groundedDashAttackEvent -= OnDashEvent;
    }
}
