using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")] 
    public Vector2[] attackMovement;
    public float counterAttackDuration;

    
    public bool isBusy { get;private set; }
    [Header("Move info")] 
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;
    
    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }

    public SkillManager skill { get;private set; }
    public GameObject sword { get; private set; }


    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idlestate { get; private set; }
    public PlayerMoveState movestate { get; private set; }
    public PlayerJumpState jumpstate { get; private set; }
    public PlayerAirState airstate { get; private set; }
    public PlayerDashState dashstate { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerWallJumpState   wallJump { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState  counterAttack { get; private set; }
    public PlayerAimSwordState aimSword { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackHoleState blackHole { get; private set; }
    #endregion
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idlestate = new PlayerIdleState(this, stateMachine, "Idle");
        movestate = new PlayerMoveState(this,stateMachine, "Move");
        jumpstate = new PlayerJumpState(this,stateMachine, "Jump");
        airstate  = new PlayerAirState(this, stateMachine, "Jump");
        dashstate = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this,stateMachine, "Jump");
        
        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        
        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackHoleState(this, stateMachine, "Jump");
    }

    protected override void Start()
    {
        base.Start();
        
        skill = SkillManager.instance;
        
        stateMachine.Initialize(idlestate);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        
        CheckForDashInput();
        
        if(Input.GetKeyDown(KeyCode.F))
            skill.crystal.CanUseSkill();
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    //协程来判断是否处于忙碌状态
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        
        yield return new WaitForSeconds(_seconds);
        
        isBusy = false;
    }

    public IEnumerator KeepZeroVelocity(float _seconds)
    {
        SetZeroVelocity();
        
        yield return new WaitForSeconds(_seconds);
    }

    //在动作最后一帧设置事件，将triggerCalled设置为true，作为攻击动作退出条件
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();   

    private void CheckForDashInput()
    {
        
        if (IsWallDetected())
            return;
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
            {
                dashDir = facingDir;
            }

            stateMachine.ChangeState(dashstate);
        }
    }
}
