using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "Player/PlayerMovementStats")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Status")]
    [Range(1f, 100f)] public float MaxHealth = 100f;

    [Header("Walk")]
    [Range(1f, 100f)] public float MaxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float GroundAcceleration = 5f;
    [Range(0.25f, 50f)] public float GroundDeceleration = 20f;
    [Range(0.25f, 50f)] public float AirAcceleration = 5f;
    [Range(0.25f, 50f)] public float AirDeceleration = 5f;

    [Header("Run")]
    [Range(1f, 100f)] public float MaxRunSpeed = 20f;

    [Header("Grounded/Collision Checks")]
    public LayerMask GroundLayer;
    public float GroundDetectionRayLength = 0.02f;
    public float HeadDetectionRayLength = 0.02f;
    [Range(0f, 1f)] public float HeadWidth = 0.75f;

    [Header("Jump")]
    public float JumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float JumpHeightCompensationFactor = 1.054f;
    public float TimeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;
    public float MaxFallSpeed = 26f;
    [Range(1, 5)] public int NumberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float ApexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float JumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;

    [Header("Dash")]
    [Range(0f, 1f)] public float DashTime = 0.11f;
    [Range(1f, 200f)] public float DashSpeed = 35f;
    [Range(0f, 1f)] public float TimeBtwDashesOnGround = 0.225f;
    [Range(0, 5)] public int NumberOfDashes = 2;
    [Range(0f, 0.5f)] public float DashDiagonallyBias = 0.4f;

    [Header("Dash Cancel Time")]
    [Range(0.01f, 5f)] public float DashGravityOnReleaseMultiplier = 1f;
    [Range(0.02f, 0.3f)] public float DashTimeForUpwardsCancel = 0.027f;

    [Header("Attack")]
    [Range(0.01f, 5f)] public float AttackRange = 1.5f;
    [Range(0f, 5f)] public float AttackTime = 3f;
    [Range(1f, 200f)] public float AttackSpeed = 40f;
    [Range(0f, 1f)] public float TimeBtwAttacksOnGround = 0.5f;
    [Range(0.1f, 3f)] public float ChargeTime = 0.6f;

    [Header("Laser")]
    [Range(1, 50)] public int LaserSpeed = 25;
    [Range(1f, 100f)] public float LaserDamage = 100f;


    [Header("Debug")]
    public bool DebugShowIsGroundedBox;
    public bool DebugShowHeadBumpBox;

    [Header("JumpVisualization Tool")]
    public bool ShowWalkJumpArc = false;
    public bool ShowRunJumpArc = false;
    public bool StopOnCollision = true;
    public bool DrawRight = true;
    [Range(5, 100)] public int ArcResolution = 20;
    [Range(0, 500)] public int VisualizationSteps = 90;

    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }
    public float AdjustedJumpHeight { get; private set; }

    public readonly Vector2[] DashDirections = new Vector2[]
    {
        new Vector2(0, 0), // No Dash
        new Vector2(1, 0), // Right
        new Vector2(-1, 0), // Left
        new Vector2(0, 1), // Up
        new Vector2(0, -1), // Down
        new Vector2(1, 1).normalized, // Right Up
        new Vector2(-1, 1).normalized, // Left Up
        new Vector2(1, -1).normalized, // Right Down
        new Vector2(-1, -1).normalized // Left Down
    };


    // Jump
    private void OnValidate()
    {
        CalculateValues();
    }

    private void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        AdjustedJumpHeight = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * AdjustedJumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;
    }
}