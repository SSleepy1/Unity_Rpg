using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    //负责存储、添加、移除物品
    /*
    ItemData：物品定义资源
    ItemObject：地面上的可拾取物体
    InventoryItem：背包里的一个物品条目
    Inventory：背包管理器，负责增删查
    */
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;   //装备列表
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;   //背包列表
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;
    
    public List<InventoryItem> stash;       //材料列表
    public Dictionary<ItemData, InventoryItem> stashDictionary;
    
    [Header("Inventory UI")] 
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashItemSlotParent;
    [SerializeField] private Transform equipmentSlotParent; 

    private UI_ItemSlot[] InventoryItemSlot;
    private UI_ItemSlot[] StashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        
        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();
        
        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();
        
        InventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        StashItemSlot = stashItemSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();

        AddStartingItems();
    }

    private void AddStartingItems()
    {
        for (int i = 0; i < startingItems.Count; i++)
        {
            AddItem(startingItems[i]);
        }
    }

    public void EquipItem(ItemData _item)   //把新装备转成装备条目，找到同类型旧装备并替换掉，然后把新装备加入装备栏。
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment; //如果 _item 不是装备类型，那 newEquipment 会是 null
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;
        
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)   //当前遍历到的装备，和新装备是不是同一个部位/类型
            {
                oldEquipment = item.Key;
            }
        }

        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);  
            AddItem(oldEquipment);
        }


        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifires();
        
        RemoveItem(_item);
        
        UpdateSlotUI();
    }

    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifires();
        }
    }

    private void UpdateSlotUI()
    {

        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                {
                    equipmentSlot[i].UpdateSlot(item.Value);
                }
            }
        }

        for(int i = 0;i<InventoryItemSlot.Length;i++)
        {
            InventoryItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            InventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {
            StashItemSlot[i].UpdateSlot(stash[i]);
        }
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment)
        {
            AddToInventory(_item);
        }
        else if(_item.itemType == ItemType.Material)
        {
            AddToStash(_item);
        }
        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if(inventoryDictionary.TryGetValue(_item,out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItme = new InventoryItem(_item);
            inventory.Add(newItme);
            inventoryDictionary.Add(_item, newItme);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if(inventoryDictionary.TryGetValue(_item,out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
            {
                value.RemoveStack();
            }
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
            {
                stashValue.RemoveStack();
            }
        }

        UpdateSlotUI();
    }

    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();   
        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("NO Enough Materials");
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                Debug.Log("NO Enough Materials");
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data);
        }
        
        AddItem(_itemToCraft);
        Debug.Log("Do Item" + _itemToCraft.name);

        return true;
    }

    public List<InventoryItem> GetEquipmentList() => equipment;
    
    public List<InventoryItem> GetStashList() => stash;
}
