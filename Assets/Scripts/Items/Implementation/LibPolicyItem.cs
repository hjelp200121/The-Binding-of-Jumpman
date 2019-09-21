using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibPolicyItem : DiscreteActiveItem
{
    private int maxCharge = 1;
    private static string name = "Liberal Policy";
    private static string description = "HeyLibs";
    private static string spritePath = "Sprites/Items/LibPolicy";
    public override string Name { get { return name; } }
    public override string Description { get { return description; } }
    public override string SpritePath { get { return spritePath; } }
    public override int MaxCharge { get { return maxCharge; } }
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