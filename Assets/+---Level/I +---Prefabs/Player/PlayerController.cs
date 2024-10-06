using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats MovementStats;
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;

    private Rigidbody2D _rb;

    // Movement
    private Vector2 _moveVelocity;
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
    }

    private void FixedUpdate()
    {
        CollisionCheck();
        Jump();

        if (_isGrounded)
        {
            Move(MovementStats.GroundAcceleration, MovementStats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            Move(MovementStats.AirAcceleration, MovementStats.AirDeceleration, InputManager.Movement);
        }
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
        else if (InputManager.JumpWasPressed && _jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < MovementStats.NumberOfJumpsAllowed)
        {
            _isFastFalling = false;
            _numberOfJumpsUsed = 2;
            InitiateJump();
        }

        // Handle air jump AFTER coyote time has lapsed (take off an extra jump so we don't get a bonus jump)
        else if (InputManager.JumpWasPressed && _jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < MovementStats.NumberOfJumpsAllowed - 1)
        {
            _numberOfJumpsUsed = 2;
            InitiateJump();
            _isFastFalling = false;
        }

        // Landed
        if ((_isJumping || _isFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
            _isJumping = false;
            _isFalling = false;
            _isFastFalling = false;
            _fastFallTime = 0f;
            _isPastApexThreshold = false;
            _numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;
        }

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
                else
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

        // Normal gravity while falling
        if (!_isGrounded && !_isJumping)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }

            VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
        }

        // Clamp fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MovementStats.MaxFallSpeed, 50f);

        _rb.velocity = new Vector2(_rb.velocity.x, VerticalVelocity);
    }

    #endregion

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {

            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.RunIsHeld)
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * MovementStats.MaxRunSpeed;
            }
            else
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * MovementStats.MaxWalkSpeed;
            }

            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            _rb.velocity = new Vector2(_moveVelocity.x, _rb.velocity.y);
        }

        else if (moveInput == Vector2.zero)
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            _rb.velocity = new Vector2(_moveVelocity.x, _rb.velocity.y);
        }
    }

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
        _jumpBufferTimer += Time.deltaTime;

        if (!_isGrounded)
        {
            _coyoteTimer += Time.deltaTime;
        }
        else
        {
            _coyoteTimer = MovementStats.JumpCoyoteTime;
        }
    }

    #endregion




}