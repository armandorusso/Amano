using UnityEngine;

public class ShieldRotation : QuickTimeComponent
{
    [SerializeField] public Transform RotationPoint;
    [SerializeField] public SpriteRenderer EnemySprite;
    private Transform _enemyTransform;
    private Transform _tetsuoTransform;

    void Start()
    {
        _enemyTransform = GetComponentInParent<ShieldSamuraiData>().transform;
        _tetsuoTransform = GetComponentInParent<ShieldSamuraiData>().TetsuoPosition;
    }

    void Update()
    {
        ApplyQuickTimeProperty();
    }

    public void OnEnemyEnabled()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            Destroy(rb);
        }
        
        // Set the parent back to the shield samurai's rotation point
        gameObject.transform.parent = RotationPoint;
        
        // Enable the shield rotation script again
        enabled = true;
        
        // Place the shield back in front of the Shield Samurai
        gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        gameObject.transform.localPosition  = new Vector2(0.5584999f, 0.047f);
        gameObject.transform.localRotation = Quaternion.identity;
    }

    private void AdjustShieldRotation()
    {
        var tetsuoTransformPosition = _tetsuoTransform.position;
        var rotationPointPosition = RotationPoint.position;

        var playerDirection = tetsuoTransformPosition - rotationPointPosition; // Get the direction between the rotation point and the player

        if (playerDirection.y > 0)
        {
            var angleBetweenPlayerDirectionAndXAxis = Mathf.Atan2(playerDirection.y,
                                                          playerDirection.x) *
                                                      Mathf
                                                          .Rad2Deg; // Get the angle between the x axis and the direction of where the player is

            angleBetweenPlayerDirectionAndXAxis = Mathf.Clamp(angleBetweenPlayerDirectionAndXAxis, 0, 180f);

            // This way, we can rotate the rotation point towards the direction of the player using that angle
            // Since the shield object is a child of the rotation point, the rotation point becomes the local axis and in turn, the shield rotates around it
            if (_enemyTransform.localScale.x < 0)
            {
                angleBetweenPlayerDirectionAndXAxis = Mathf.Clamp(angleBetweenPlayerDirectionAndXAxis, 90f, 180f);
            }

            RotationPoint.rotation = Quaternion.Euler(0f, 0f, angleBetweenPlayerDirectionAndXAxis);
            // If the enemy is facing in the opposite direction, what ends up happening is the shield ends up in the opposite direction of where it should be
            // This if statement prevents that from happening
        }

    }

    public override void ApplyQuickTimeProperty()
    {
        AdjustShieldRotation();
    }
}
