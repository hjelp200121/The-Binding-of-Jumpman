using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibPolicyItem : DiscreteActiveItem
{
    public override bool Use(PlayerController player)
    {
        if (base.Use(player))
        {
            player.Heal(player.maxHealth / 2);
            return true;
        }
        else
        {
            return false;
        }
    }
}