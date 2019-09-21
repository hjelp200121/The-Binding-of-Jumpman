using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTreasureItem : PassiveItem
{
    private static string name = "Test treasure item";
    private static string description = "also no description";
    private static string spritePath = "Sprites/Items/BigMushroom";
    public override string Name { get { return name; } }
    public override string Description { get { return description; } }
    public override string SpritePath { get { return spritePath; } }
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.fireDelay *= 0.2f;
    }
}
