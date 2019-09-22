using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lawnmower : DiscreteActiveItem
{
    public override bool Use(PlayerController player)
    {

        if (base.Use(player))
        {
            player.ActivateLawnmower();
            return true;
        }
        else
        {
            return false;
        }
    }
}