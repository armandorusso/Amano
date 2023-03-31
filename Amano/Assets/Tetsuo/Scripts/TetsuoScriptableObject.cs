using UnityEngine;

[CreateAssetMenu(fileName = "TetsuoScriptableObject", menuName = "ScriptableObject/Tetsuo")]
public class TetsuoScriptableObject : ScriptableObject
{
    [Header("Movement")] 
    [SerializeField] public float MaxRunSpeed;
    [SerializeField] public float MaxAccelSpeed;
    [SerializeField] public float MaxAirAccelSpeed;
    [SerializeField] public float MaxDeAccelSpeed;
    [SerializeField] public float MaxDeAccelAirSpeed;
    [SerializeField] public float RunLerpAmount;
    [SerializeField] public bool ConserveMomentum;
    [SerializeField] public float MaxFallSpeed;
    [SerializeField] public float FallGravityMultiplier;
    [SerializeField] public float _speed = 6f;
    
    [Header("Jumping")] 
    [SerializeField] public float ShortHopFallMultiplier;
    [SerializeField] public float FullHopFallMultiplier;
    [SerializeField] public float JumpHangTime;
    [SerializeField] public float _jumpingPower = 12f;
    [SerializeField] public float coyoteTime = 0.2f;
    [SerializeField] public float coyoteTimeCounter;
    [SerializeField] public float jumpBufferTime = 0.2f;
     
    [Header("Wall Sliding")]
    [SerializeField] public float wallSlideSpeed = 0f;
    [SerializeField] public LayerMask wallLayer;
    
    [Header("Wall Jumping")]
    [SerializeField] public Vector2 wallJumpingDirection;
    [SerializeField] public float wallJumpForce;
    [SerializeField] public float wallJumpingTime;
    
    [Header("Dash Attack")]
    [SerializeField] public float dashPower;
    [SerializeField] public float dashTime;
    [SerializeField] public float dashCooldown;
    [SerializeField] public float dashCooldownOnGround;
    
    [Header("Wall Sticking")]
    public float _WallStickingTimer = 2f;
}
