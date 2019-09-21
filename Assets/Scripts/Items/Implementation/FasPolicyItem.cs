using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasPolicyItem : DiscreteActiveItem
{
    private int maxCharge = 1;
    private static string name = "Fascist Policy";
    private static string description = "minkus";
    private static string spritePath = "Sprites/Items/FasPolicy";
    public override string Name { get { return name; } }
    public override string Description { get { return description; } }
    public override string SpritePath { get { return spritePath; } }
    public override int MaxCharge { get { return maxCharge; } }
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
