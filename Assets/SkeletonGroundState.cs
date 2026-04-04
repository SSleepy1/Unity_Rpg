using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundState : EnemyState
{
    protected Enemy_Skeleton enemy;
    public SkeletonGroundState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Skeleton enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetcted())
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }


    public override void Exit()
    {
        base.Exit();
    }
}
