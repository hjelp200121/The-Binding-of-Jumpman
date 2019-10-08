using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroItem : DiscreteActiveItem
{
    public override bool Use(PlayerController player)
    {
        if (base.Use(player))
        {
            foreach (DungeonObject dungeonObject in player.currentRoom.contents.objects)
            {
                ItemPedestal itemPedestal = dungeonObject as ItemPedestal;
                if (itemPedestal != null && itemPedestal.item != null)
                {
                    Item itemPrefab = itemPedestal.pool.GetRandomItem();
                    if (itemPrefab != null)
                    {
                        Destroy(itemPedestal.item.gameObject);
                        itemPedestal.item = Item.InstantiateItem(itemPrefab, itemPedestal.itemParent);
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}