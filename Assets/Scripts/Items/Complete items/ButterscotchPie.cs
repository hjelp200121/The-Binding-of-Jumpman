using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterscotchPie : DiscreteActiveItem
{
    public override bool Use(PlayerController player)
    {
        if (base.Use(player))
        {
            player.Heal(player.maxHealth);
            player.activeItem = null;
            Destroy(gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }
}