using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccessfulCounterAttack",false);
    }
    public override void Update()
    {
        base.Update();
        
        player.SetZeroVelocity();
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);   
        
        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())   //Enemy_Skeleton中这个函数返回bool值，并切换statemachine为stunnedState
                {
                    stateTimer = 10;    //给一个大值，避免因为状态时间退出反击状态时，没播放完反击动画
                    player.anim.SetBool("SuccessfulCounterAttack",true);
                }
            }
        }

        if (stateTimer < 0 || triggerCalled)
        {
            stateMachine.ChangeState(player.idlestate);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

}
