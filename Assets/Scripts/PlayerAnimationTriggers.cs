using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();    //获取父类组件

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }
}
