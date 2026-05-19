using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    //场景中实际掉落的物品
    [SerializeField] private ItemData itemData;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void SetupVisuals()
    {
        if (itemData == null)
            return;
        
        GetComponent<SpriteRenderer>().sprite = itemData.Icon;
        gameObject.name = "Item object - " + itemData.itemName;
    }

    public void SetupItem(ItemData _itemData,Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;
        
        SetupVisuals();
    }

    public void PickupItem()
    {
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
