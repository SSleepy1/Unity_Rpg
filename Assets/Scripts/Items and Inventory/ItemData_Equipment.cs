using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,     //武器
    Armor,      //护甲
    Amulet,     //护符
    Flask       //药瓶
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Major stats")]
    public int strength;//每一点力量提升百分之一攻击力和百分之一爆伤
    public int agility;//每一点敏捷可以提升百分之一闪避和百分之一暴击
    public int intelligence;//每一点魔力可以提升一法强和3魔抗
    public int vitality;//每一点生命可以提升3生命值
    
    [Header("Offensive stats")]
    public int damage;
    public int critChance;
    public int critPower;
    
    [Header("Defensive stats")]
    public int maxHealth;
    public int armor;//护甲
    public int evasion;//闪避率
    public int magicResistance;//魔抗

    [Header("Maigc stats")] 
    public int fireDamage;
    public int iceDamage;
    public int lightingDamage;

    [Header("Craft requirements")] 
    public List<InventoryItem> craftingMaterials;

    public void AddModifires()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        
        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);
        
        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);
        
        playerStats.maxHealth.AddModifier(maxHealth);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);
        
        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightingDamage.AddModifier(lightingDamage);
    }

    public void RemoveModifires()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        
        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);
        
        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);
        
        playerStats.maxHealth.RemoveModifier(maxHealth);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);
        
        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightingDamage.RemoveModifier(lightingDamage);
    }
}
