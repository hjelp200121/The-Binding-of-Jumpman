using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasPolicyItem : DiscreteActiveItem
{
    public override bool Use(PlayerController player)
    {
        if (base.Use(player))
        {
            player.TakeDamage(player.maxHealth / 2, player);
            return true;
        }
        else
        {
            return false;
        }
    }
}
