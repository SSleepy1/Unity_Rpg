using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;
    [Header("Major stats")]
    public Stat strength;//每一点力量提升百分之一攻击力和百分之一爆伤
    public Stat agility;//每一点敏捷可以提升百分之一闪避和百分之一暴击
    public Stat intelligence;//每一点魔力可以提升一法强和3魔抗
    public Stat vitality;//每一点生命可以提升3生命值
    
    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;
    
    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;//护甲
    public Stat evasion;//闪避率
    public Stat magicResistance;//魔抗

    [Header("Maigc stats")] 
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited;//持续伤害
    public bool isChilled;//减防20%
    public bool isShocked;//使对象攻击成功率降低20%

    [SerializeField] private float alimentsDuration = 4;
    private float igniteTimer;
    private float chilledTimer;
    private float shockedTimer;
    
    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;

    private bool isDead;

    public int currentHealth;

    public System.Action onHealthChanged;
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        
        onHealthChanged?.Invoke();
        
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        igniteTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        
        igniteDamageTimer -= Time.deltaTime;

        if (igniteTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;
        
        // if (igniteDamageTimer < 0 && isIgnited)
        // {
        //     if (isDead)
        //         return;
        //     
        //     Debug.Log("Ignite" + igniteDamage);
        //
        //     currentHealth -= igniteDamage;
        //
        //     if (currentHealth < 0)
        //     {
        //         Die();
        //     }
        //
        //     igniteDamageTimer = igniteDamageCooldown;
        // }
        if (igniteDamageTimer < 0 && isIgnited)
        {
            if (isDead)
                return;
            
            Debug.Log("Ignite" + igniteDamage);

            DecreaseHealthBy(igniteDamage);

            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        //DoPhysicalDamage(_targetStats);
        DoMagicalDamage(_targetStats);
    }

    private void DoPhysicalDamage(CharacterStats _targetStats)
    {
        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
    }

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();
        
        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage);

        int maxDamage = Mathf.Max(_fireDamage, _iceDamage, _lightingDamage);
        
        if (maxDamage <= 0)
        {
            return;
        }
        
        bool canApplyIgnite = false;
        bool canApplyChill = false;
        bool canApplyShock = false;
        
        GetRandomDominantElement(_fireDamage, _iceDamage, _lightingDamage,maxDamage,ref canApplyIgnite, ref canApplyChill, ref canApplyShock);

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
        
        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }

    }

    private static void GetRandomDominantElement(int _fireDamage, int _iceDamage, int _lightingDamage,int maxDamage
        ,ref bool canApplyIgnite, ref bool canApplyChill, ref bool canApplyShock)
    {
        int equalCount = 0;
        
        if (_fireDamage == maxDamage) equalCount++;
        if (_iceDamage == maxDamage) equalCount++;
        if (_lightingDamage == maxDamage) equalCount++;
        
        int randomIndex = Random.Range(0, equalCount);
        int currentIndex = 0;
        
        if (_fireDamage == maxDamage)
        {
            if (currentIndex == randomIndex)
            {
                canApplyIgnite = true;
            }

            currentIndex++;
        }
        
        if (_iceDamage == maxDamage)
        {
            if (currentIndex == randomIndex)
            {
                canApplyChill = true;
            }
            currentIndex++;
        }
        
        if (_lightingDamage == maxDamage)
        {
            if (currentIndex == randomIndex)
            {
                canApplyShock = true;
            }
        }
    }

    private static int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage  = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public void ApplyAilments(bool _ignite,bool _chill,bool _shock)
    {
        // if (isIgnited || isChilled || isShocked)
        // {
        //     return;
        // }

        if (_ignite)
        {
            isIgnited = _ignite;
            igniteTimer = alimentsDuration;
            
            fx.IgniteFxFor(alimentsDuration);
        }

        if (_chill)
        {
            isChilled = _chill;
            chilledTimer = alimentsDuration;
            
            fx.ChillFxFor(alimentsDuration);
        }

        if (_shock)
        {
            isShocked = _shock;
            shockedTimer = alimentsDuration;
            
            fx.ShockFxFor(alimentsDuration);
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    
    public virtual void TakeDamage(int _damage)  //针对既造成伤害，又显示受击动画的函数
    {
        if (isDead)
            return;
        
        DecreaseHealthBy(_damage);

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    protected virtual void DecreaseHealthBy(int _damage)    //针对只造成伤害，不显示受击动画的函数
    {
        currentHealth -= _damage;

        if (onHealthChanged != null)
            onHealthChanged();
    }

    public virtual void Die()
    {
        
    }
    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
        {
            totalEvasion = Mathf.RoundToInt(totalEvasion * 1.2f);
        }

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }

        return false;
    }
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();
        }


        //将伤害限制在0~max之间，防止出现负数回血的情况
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();
        
        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 3;
    }
}
