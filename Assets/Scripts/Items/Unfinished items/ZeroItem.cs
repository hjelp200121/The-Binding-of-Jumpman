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
                ItemPedestal ItemPedestal = dungeonObject as ItemPedestal;
                if (ItemPedestal != null)
                {
                    Destroy(ItemPedestal.item.gameObject);
                    ItemPedestal.item = Instantiate<Item>(ItemPedestal.pool.GetRandomItem(), ItemPedestal.itemParent);
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