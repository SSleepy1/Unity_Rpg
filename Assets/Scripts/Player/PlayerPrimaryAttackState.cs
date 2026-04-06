using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCounter;

    private float lastTimeAttacked;
    private float comboWindow = 2;
    
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        float currentXInput = Input.GetAxisRaw("Horizontal");

        if (comboCounter > 2 || Time.time >= lastTimeAttacked +  comboWindow)
            comboCounter = 0;
        
        player.anim.SetInteger("ComboCounter",comboCounter);
        //player.anim.speed = 1.2f;  //加攻速（加快动画播放）
        
        //修复攻击转变方向时不转向的bug
        float attackDir = player.facingDir;

        if (currentXInput != 0)
        {
            attackDir = Mathf.Sign(currentXInput);

            //但如果攻击位移很小或为0，SetVelocity中的 FlipController可能不会触发翻转，所以手动调用一次是更保险的做法。
            player.FlipController(attackDir);
        }


        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir,player.attackMovement[comboCounter].y);   //使每段攻击向前位移一段距离

        stateTimer = .1f;   //保证攻击时不会立即停下，过度更自然
    }
    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);
        //player.anim.speed = 1;
        
        comboCounter++;
        lastTimeAttacked = Time.time;
    }
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            player.SetZeroVelocity();
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idlestate);
        }
    }

}
