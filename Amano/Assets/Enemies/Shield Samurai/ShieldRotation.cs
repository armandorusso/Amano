using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRotation : MonoBehaviour
{
    [SerializeField] public Transform RotationPoint;
    private Transform _enemyTransform;
    private Transform _tetsuoTransform;
    
    void Start()
    {
        _enemyTransform = GetComponentInParent<ShieldSamuraiData>().transform;
        _tetsuoTransform = GetComponentInParent<ShieldSamuraiData>().TetsuoPosition;
    }

    void Update()
    {
        AdjustShieldRotation();
    }

    private void AdjustShieldRotation()
    {
        var tetsuoTransformPosition = _tetsuoTransform.position;
        var rotationPointPosition = RotationPoint.position;

        var playerDirection = tetsuoTransformPosition - rotationPointPosition; // Get the direction between the rotation point and the player

        var angleBetweenPlayerDirectionAndXAxis = Mathf.Atan2(playerDirection.y,
            playerDirection.x) * Mathf.Rad2Deg; // Get the angle between the x axis and the direction of where the player is
        
        // Clamp the angle so the shield doesn't end up rotating towards the ground
        Mathf.Clamp(angleBetweenPlayerDirectionAndXAxis, 0f, 180f);

        // This way, we can rotate the rotation point towards the direction of the player using that angle
        // Since the shield object is a child of the rotation point, the rotation point becomes the local axis and in turn, the shield rotates around it
        RotationPoint.rotation = Quaternion.Euler(0f, 0f, angleBetweenPlayerDirectionAndXAxis);
        
        // If the enemy is facing in the opposite direction, what ends up happening is the shield ends up in the opposite direction of where it should be
        // This if statement prevents that from happening
        if (_enemyTransform.localScale.x != RotationPoint.localScale.x)
        {
            RotationPoint.localScale = _enemyTransform.localScale;
        }
    }
}
