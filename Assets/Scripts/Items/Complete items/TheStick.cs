using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStick : DiscreteActiveItem
{
    public override bool Use(PlayerController player)
    {

        if (base.Use(player))
        {
            foreach (DungeonObject dungeonObject in player.currentRoom.contents.objects) {
                Enemy enemy = dungeonObject as Enemy;
                if (enemy != null) {
                    enemy.TakeDamage(player.damage);
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