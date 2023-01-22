using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderEventManager : MonoBehaviour
{
    [SerializeField] public Material vanishingMaterial;

    private void Awake()
    {
        TeleportAbility.VanishingEvent += OnTeleportEvent;
    }

    private void OnTeleportEvent(object sender, TeleportAbility.VanishingFxEventArgs e)
    {
        var teleportSubject = e.objectBeingTeleported1;
        var teleportedObject = e.objectBeingTeleported2;

        StartCoroutine(PlayVanishingVFX(teleportSubject, teleportedObject));
    }

    private IEnumerator PlayVanishingVFX(GameObject subject, GameObject teleportedObject)
    {
        var originalMaterialSubject = subject.GetComponent<SpriteRenderer>().material;
        var originalMaterialBeingTeleported = teleportedObject.GetComponent<SpriteRenderer>().material;
        subject.GetComponent<SpriteRenderer>().material = vanishingMaterial;
        teleportedObject.GetComponent<SpriteRenderer>().material = vanishingMaterial;
        yield return new WaitForSeconds(0.8f);
        subject.GetComponent<SpriteRenderer>().material = originalMaterialSubject;
        teleportedObject.GetComponent<SpriteRenderer>().material = originalMaterialBeingTeleported;
    }
}
