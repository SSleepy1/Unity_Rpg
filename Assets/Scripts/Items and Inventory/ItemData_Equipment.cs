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
}
