using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill : Skill
{
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float blackholeDuration;
    [Space]
    [SerializeField] private GameObject BlackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    private Blackhole_Skill_Controller currentBlackhole;
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(BlackHolePrefab,player.transform.position,Quaternion.identity);
        
        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();
        
        currentBlackhole.SetupBlackhole(maxSize,growSpeed,shrinkSpeed,amountOfAttacks,attackCooldown,blackholeDuration);
    }

    public bool SkillCompleted()
    {
        if (!currentBlackhole)
            return false;

        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }
        
        return false;
    }

    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }
}
