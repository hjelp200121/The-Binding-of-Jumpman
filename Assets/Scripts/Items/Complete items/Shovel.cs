using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : DiscreteActiveItem
{
    public override bool Use(PlayerController player) {

        if (base.Use(player)) {
            foreach (DungeonObject dungeonObject in player.currentRoom.contents.objects) {
                Enemy enemy = dungeonObject as Enemy;
                if (enemy != null) {
                    if (Vector2.Distance(enemy.transform.position, player.transform.position) < 2)
                    {
                        enemy.TakeDamage(10);
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