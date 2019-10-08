using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnoyingDog : DiscreteActiveItem
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
                    if(Random.Range(0,101) > 66)
                    {
                        itemPedestal.item = Instantiate<Item>(itemPedestal.pool.GetRandomItem(), itemPedestal.itemParent);
                    } 
                    player.keyCount += Random.Range(-2, 3);
                    player.bombCount += Random.Range(-2, 3);

                    player.maxHealth += Random.Range(-1, 2);
                    player.health += Random.Range(-2, 3);
                    player.damage *= Random.Range(0.85f, 1.5f);
                    player.fireDelay *= Random.Range(0.75f, 1.3f);
                    player.shotTimer *= Random.Range(0.85f, 1.5f);
                    player.shotSpeed *= Random.Range(0.85f, 1.5f);
                    player.accuracy *= Random.Range(0.75f, 1.3f);
                    player.speed *= Random.Range(0.85f, 1.5f);
                    player.acceleration *= Random.Range(0.85f, 1.5f);
                    player.invincibilityOnDamage *= Random.Range(0.85f, 1.5f);
                    player.UpdateUI();
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