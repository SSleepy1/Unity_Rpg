using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    
    protected EnemyStateMachine stateMachine;
    protected Enemy EnemyBase;
    protected Rigidbody2D rb;

    private string animBoolName;
    
    protected float stateTimer;
    protected bool triggerCalled;

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.EnemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }
    
    public virtual void Enter()
    {
        triggerCalled = false;
        rb = EnemyBase.rb;
        EnemyBase.anim.SetBool(animBoolName, true);
    }

    public virtual void Exit()
    {
        EnemyBase.anim.SetBool(animBoolName, false);
        EnemyBase.AssignLastAnimName(animBoolName);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }   
}
