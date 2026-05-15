using UnityEngine;

public enum ItemType
{
    Material,
    Equipment
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject    
{
    //道具的数据定义
    public ItemType itemType;
    public string itemName;
    public Sprite Icon;
}
