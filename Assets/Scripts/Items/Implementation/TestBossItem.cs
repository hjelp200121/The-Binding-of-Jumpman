using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossItem : PassiveItem
{
    private static string name = "Test boss item";
    private static string description = "no description";
    private static string spritePath = "Sprites/Items/0";
    public override string Name { get { return name; } }
    public override string Description { get { return description; } }
    public override string SpritePath { get { return spritePath; } }
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.speed *= 1.5f;
    }
}
