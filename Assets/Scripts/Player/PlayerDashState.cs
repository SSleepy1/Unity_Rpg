using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skill.clone.CreateCloneOnDashStart();
        
        stateTimer = player.dashDuration;
    }
    public override void Update()
    {
        base.Update();

        if (!player.IsGroundedDetected() && player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlide);
        }

        player.SetVelocity(player.dashSpeed * player.dashDir,0);
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idlestate);
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.skill.clone.CreateClongOnDashOver();
        player.SetVelocity(0,rb.velocity.y);
    }

}
