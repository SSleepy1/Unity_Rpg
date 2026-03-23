using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Attack details")] 
    public Vector2[] attackMovement;
    public bool isBusy { get;private set; }
    [Header("Move info")] 
    public float moveSpeed = 12f;
    public float jumpForce;
    
    [Header("Dash info")]
    [SerializeField] private float dashCooldown;
    private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }

    [Header("Collision info")] 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    public int facingDir { get; private set; } = 1;
    private bool facingRight = true;

    #region Components
    public Animator anim { get;private set; }
    
    public Rigidbody2D rb { get;private set; }
    
    #endregion
    
    
    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idlestate { get; private set; }
    public PlayerMoveState movestate { get; private set; }
    public PlayerJumpState jumpstate { get; private set; }
    public PlayerAirState airstate { get; private set; }
    public PlayerDashState dashstate { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerWallJumpState   wallJump { get; private set; }
    
    public PlayerPrimaryAttackState primaryattack { get; private set; }
    #endregion
    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        idlestate = new PlayerIdleState(this, stateMachine, "Idle");
        movestate = new PlayerMoveState(this,stateMachine, "Move");
        jumpstate = new PlayerJumpState(this,stateMachine, "Jump");
        airstate  = new PlayerAirState(this, stateMachine, "Jump");
        dashstate = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this,stateMachine, "Jump");

        primaryattack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        stateMachine.Initialize(idlestate);
    }
    //协程来判断是否处于忙碌状态
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        
        yield return new WaitForSeconds(_seconds);
        
        isBusy = false;
    }

    private void Update()
    {
        stateMachine.currentState.Update();
        
        CheckForDashInput();
    }

    //在攻击动作最后一帧设置事件，将triggerCalled设置为true，作为攻击动作退出条件
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();   

    private void CheckForDashInput()
    {
        dashUsageTimer -= Time.deltaTime;
        
        if (IsWallDetected())
            return;
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer < 0)
        {
            dashUsageTimer = dashCooldown;
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
            {
                dashDir = facingDir;
            }

            stateMachine.ChangeState(dashstate);
        }
    }
    #region Velocity
    public void ZeroVelocity() => rb.velocity = new Vector2(0, 0);


    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion
    #region Collision
    public bool IsGroundedDetected() =>
        Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    
    public bool IsWallDetected() =>Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position,new Vector3(groundCheck.position.x,groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position,new Vector3(wallCheck.position.x + wallCheckDistance,wallCheck.position.y));
    }
    #endregion
    #region Flip
    public void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0,180,0);
    }

    public void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }
    #endregion
}
