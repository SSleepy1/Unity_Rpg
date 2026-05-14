using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject    
{
    //道具的数据定义
    public string itemName;
    public Sprite Icon;
}
