using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    
    protected EnemyStateMachine stateMachine;
    protected Enemy EnemyBaseBase;

    private string animBoolName;
    
    protected float stateTimer;
    protected bool triggerCalled;

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.EnemyBaseBase = _enemyBase;
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
        EnemyBaseBase.anim.SetBool(animBoolName, true);
    }

    public virtual void Exit()
    {
        EnemyBaseBase.anim.SetBool(animBoolName, false);
    }
}
