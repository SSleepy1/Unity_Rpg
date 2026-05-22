using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
   [Header("Player's drop")] 
   [SerializeField] private float chanceToLooseItems;
   [SerializeField] private float chanceToLooseMaterials;

   public override void GenerateDrop()
   {
      Inventory inventory = Inventory.instance;
      
      List<InventoryItem> itemsToUnequip = new List<InventoryItem>();   //用来把遍历和修改分开，避免遍历集合时直接改集合带来的错误
      List<InventoryItem> materialsToLoose = new List<InventoryItem>();

      foreach (InventoryItem item in inventory.GetEquipmentList())
      {
         if (Random.Range(0, 100) <= chanceToLooseItems)
         {
            DropItem(item.data);
            itemsToUnequip.Add(item);
         }
      }

      for (int i = 0; i < itemsToUnequip.Count; i++)
      {
         inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
      }

      foreach (InventoryItem item in inventory.GetStashList())
      {
         if (Random.Range(0, 100) <= chanceToLooseMaterials)
         {
            DropItem(item.data);
            itemsToUnequip.Add(item);
         }
      }

      for (int i = 0; i < materialsToLoose.Count; i++)
      {
         inventory.RemoveItem(materialsToLoose[i].data);
      }
   }
}
