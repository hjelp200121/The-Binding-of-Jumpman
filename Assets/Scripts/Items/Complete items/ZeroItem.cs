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
                    Destroy(itemPedestal.item.gameObject);
                    itemPedestal.item = Instantiate<Item>(itemPedestal.pool.GetRandomItem(), itemPedestal.itemParent);
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