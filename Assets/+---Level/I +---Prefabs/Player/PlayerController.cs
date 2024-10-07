using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public PlayerMovementStats MovementStats;
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;

    private Rigidbody2D _rb;

    // Movement
    public float HorizontalVelocity { get; private set; }
    private bool _isFacingRight;

    // Collision Check
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private bool _isGrounded;
    private bool _bumpedHead;

    // Jump vars
    public float VerticalVelocity { get; private set; }
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    // Apex vars
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    // Jump Buffer vars
    private float _jumpBufferTimer;
    private bool _isJumpReleasedDuringBuffer;

    // Jump Coyote Time vars
    private float _coyoteTimer;

    // Dash vars
    private bool _isDashing;
    private bool _isAirDashing;
    private float _dashTimer;
    private float _dashOnGroundTimer;
    private int _numberOfDashesUsed;
    private Vector2 _dashDirection;
    private bool _isDashFastFalling;
    private float _dashFastFallTime;
    private float _dashFastFallReleaseSpeed;

    #endregion

    private void Awake()
    {
        _isJumping = false;
        _isFacingRight = true;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CountTimers();
        JumpChecks();
        DashCheck();
        LandCheck();
    }

    private void FixedUpdate()
    {
        CollisionCheck();
        Jump();
        Dash();
        Fall();

        if (_isGrounded)
        {
            Move(MovementStats.GroundAcceleration, MovementStats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            Move(MovementStats.AirAcceleration, MovementStats.AirDeceleration, InputManager.Movement);
        }

        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        // Clamp fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MovementStats.MaxFallSpeed, 50f);

        _rb.velocity = new Vector2(HorizontalVelocity, VerticalVelocity);
    }

    #region Movement

    #region Jump

    private void JumpChecks()
    {
        // When we press the jump button
        if (InputManager.JumpWasPressed)
        {
            _jumpBufferTimer = MovementStats.JumpBufferTime;
            _isJumpReleasedDuringBuffer = false;
        }

        // When we release the jump button
        if (InputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _isJumpReleasedDuringBuffer = true;
            }

            if (_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MovementStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        // Initiate jump with jump buffering and coyote time
        if (InputManager.JumpWasPressed && _jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            _numberOfJumpsUsed = 1;
            InitiateJump();

            if (_isJumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        // Double jump
        else if (InputManager.JumpWasPressed && _jumpBufferTimer > 0f && (_isJumping || _isAirDashing || _isDashFastFalling) && _numberOfJumpsUsed < MovementStats.NumberOfJumpsAllowed)
        {
            _isFastFalling = false;
            _numberOfJumpsUsed = 2;
            InitiateJump();

            if (_isDashFastFalling)
            {
                _isDashFastFalling = false;
            }
        }

        // Handle air jump AFTER coyote time has lapsed (take off an extra jump so we don't get a bonus jump)
        else if (InputManager.JumpWasPressed && _jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < MovementStats.NumberOfJumpsAllowed - 1)
        {
            _numberOfJumpsUsed = 2;
            InitiateJump();
            _isFastFalling = false;
        }

    }

    private void ResetJumpValues()
    {
        _isJumping = false;
        _isFalling = false;
        _isFastFalling = false;
        _fastFallTime = 0f;
        _isPastApexThreshold = false;
        _numberOfJumpsUsed = 0;
    }

    private void InitiateJump()
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }

        _jumpBufferTimer = 0f;
        VerticalVelocity = MovementStats.InitialJumpVelocity;

    }

    private void Jump()
    {
        // Apply gravity while jumping
        if (_isJumping)
        {
            // Check for head bump
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }

            // Gravity on ascending
            if (VerticalVelocity >= 0f)
            {
                // Apex controls
                _apexPoint = Mathf.InverseLerp(MovementStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > MovementStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < MovementStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }

                // Gravity on ascending but not past apex threshold
                else if (!_isFastFalling)
                {
                    VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }

            }

            // Gravity on descending
            else if (!_isFastFalling)
            {
                VerticalVelocity += MovementStats.Gravity * MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (VerticalVelocity < 0f)
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                }
            }
        }

        // Jump cut
        if (_isFastFalling)
        {
            if (_fastFallTime >= MovementStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MovementStats.Gravity * MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_fastFallTime < MovementStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / MovementStats.TimeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }
    }

    #endregion

    #region Move

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (!_isDashing)
        {

            if (moveInput != Vector2.zero)
            {

                TurnCheck(moveInput);

                float targetVelocity = 0f;
                if (InputManager.RunIsHeld)
                {
                    targetVelocity = moveInput.x * MovementStats.MaxRunSpeed;
                }
                else
                {
                    targetVelocity = moveInput.x * MovementStats.MaxWalkSpeed;
                }

                HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            }

            else if (moveInput == Vector2.zero)
            {
                HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0f, deceleration * Time.fixedDeltaTime);
            }
        }

    }

    #endregion

    #region Land/Fall

    private void LandCheck()
    {
        // Landed
        if ((_isJumping || _isFalling || _isDashFastFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
            ResetJumpValues();
            ResetDashes();

            VerticalVelocity = Physics2D.gravity.y;

            if (_isDashFastFalling && _isGrounded)
            {
                ResetDashValues();
                return;
            }

            ResetDashValues();
        }
    }

    private void Fall()
    {
        // Normal gravity while falling
        if (!_isGrounded && !_isJumping)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }

            VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
        }
    }

    #endregion

    #region Dash

    private void DashCheck()
    {
        if (InputManager.DashWasPressed)
        {
            // ground dash
            if (_isGrounded && _dashOnGroundTimer <= 0f && !_isDashing)
            {
                InitiateDash();
            }

            // air dash
            else if (!_isGrounded && _numberOfDashesUsed < MovementStats.NumberOfDashes && !_isDashing)
            {
                _isAirDashing = true;
                InitiateDash();
            }
        }
    }

    private void InitiateDash()
    {
        _dashDirection = InputManager.Movement;

        Vector2 closestDirection = Vector2.zero;
        float minDistance = Vector2.Distance(_dashDirection, MovementStats.DashDirections[0]);

        for (int i = 0; i < MovementStats.DashDirections.Length; i++)
        {
            // skip if we hit it bang on
            if (_dashDirection == MovementStats.DashDirections[i])
            {
                closestDirection = _dashDirection;
                break;
            }

            float distance = Vector2.Distance(_dashDirection, MovementStats.DashDirections[i]);

            // check if this is a diagonal direction and apply bias
            bool isDiagonal = (Mathf.Abs(MovementStats.DashDirections[i].x) == 1 && Mathf.Abs(MovementStats.DashDirections[i].y) == 1);
            if (isDiagonal)
            {
                distance -= MovementStats.DashDiagonallyBias;
            }

            else if (distance < minDistance)
            {
                minDistance = distance;
                closestDirection = MovementStats.DashDirections[i];
            }
        }

        // handle dash direction with NO input
        if (closestDirection == Vector2.zero)
        {
            if (_isFacingRight)
            {
                closestDirection = Vector2.right;
            }
            else
            {
                closestDirection = Vector2.left;
            }
        }

        _dashDirection = closestDirection;
        _numberOfDashesUsed++;
        _isDashing = true;
        _dashTimer = 0f;
        _dashOnGroundTimer = MovementStats.TimeBtwDashesOnGround;

        ResetJumpValues();
    }

    private void Dash()
    {
        if (_isDashing)
        {
            // stop the dash after the timer
            _dashTimer += Time.fixedDeltaTime;
            if (_dashTimer >= MovementStats.DashTime)
            {
                if (_isGrounded)
                {
                    ResetDashes();
                }

                _isAirDashing = false;
                _isDashing = false;

                if (!_isJumping)
                {
                    _dashFastFallTime = 0f;
                    _dashFastFallReleaseSpeed = VerticalVelocity;

                    if (!_isGrounded)
                    {
                        _isDashFastFalling = true;
                    }
                }

                return;
            }

            HorizontalVelocity = MovementStats.DashSpeed * _dashDirection.x;

            if (_dashDirection.y != 0f || _isAirDashing)
            {
                VerticalVelocity = MovementStats.DashSpeed * _dashDirection.y;
            }
        }

        // handle dash cut time
        else if (_isDashFastFalling)
        {
            if (VerticalVelocity > 0f)
                if (_dashFastFallTime < MovementStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity = Mathf.Lerp(_dashFastFallReleaseSpeed, 0f, (_dashFastFallTime / MovementStats.DashTimeForUpwardsCancel));
                }
                else if (_dashFastFallTime >= MovementStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity += MovementStats.Gravity * MovementStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }

            _dashFastFallTime += Time.fixedDeltaTime;
        }

        else
        {
            VerticalVelocity += MovementStats.Gravity * MovementStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
        }
    }

    private void ResetDashValues()
    {
        _isDashFastFalling = false;
        _dashOnGroundTimer = -0.01f;
    }

    private void ResetDashes()
    {
        _numberOfDashesUsed = 0;

    }

    #endregion

    private void TurnCheck(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !_isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && _isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localeScale = transform.localScale;
        localeScale.x *= -1f;
        transform.localScale = localeScale;
    }

    #endregion


    #region Collision Check

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, MovementStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MovementStats.GroundDetectionRayLength, MovementStats.GroundLayer);
        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

        // #region Debug Visualization
        // if (MovementStats.DebugShowIsGroundedBox)
        // {
        //     Color rayColor;
        //     if (_isGrounded)
        //     {
        //         rayColor = Color.green;
        //     }
        //     else
        //     {
        //         rayColor = Color.red;
        //     }

        //     Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MovementStats.GroundDetectionRayLength, rayColor);
        //     Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MovementStats.GroundDetectionRayLength, rayColor);
        //     Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - MovementStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        // }

        // #endregion
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_bodyColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_bodyColl.bounds.size.x * MovementStats.HeadWidth, MovementStats.HeadDetectionRayLength);
        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MovementStats.HeadDetectionRayLength, MovementStats.GroundLayer);

        if (_headHit.collider != null)
        {
            _bumpedHead = true;
        }
        else
        {
            _bumpedHead = false;
        }

        // #region Debug Visualization
        // if (MovementStats.DebugShowHeadBumpBox)
        // {
        //     float headWidth = MovementStats.HeadWidth;

        //     Color rayColor;
        //     if (_bumpedHead)
        //     {
        //         rayColor = Color.green;
        //     }
        //     else
        //     {
        //         rayColor = Color.red;
        //     }

        //     Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * MovementStats.HeadDetectionRayLength, rayColor);
        //     Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * MovementStats.HeadDetectionRayLength, rayColor);
        //     Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + MovementStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        // }

        // #endregion
    }

    private void CollisionCheck()
    {
        IsGrounded();
        BumpedHead();
    }


    #endregion

    #region Timers

    private void CountTimers()
    {

        // jump buffer
        _jumpBufferTimer += Time.deltaTime;

        // jump coyote time
        if (!_isGrounded)
        {
            _coyoteTimer += Time.deltaTime;
        }
        else
        {
            _coyoteTimer = MovementStats.JumpCoyoteTime;
        }

        // dash timer
        if (_isGrounded)
        {
            _dashOnGroundTimer -= Time.deltaTime;
        }
    }

    #endregion




}