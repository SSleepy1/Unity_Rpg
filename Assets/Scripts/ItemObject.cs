using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    //场景中实际掉落的物品
    private SpriteRenderer sr;
    
    [SerializeField] private ItemData itemData;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = itemData.Icon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Inventory.instance.AddItem(itemData);
            Destroy(gameObject);
        }
    }
}
