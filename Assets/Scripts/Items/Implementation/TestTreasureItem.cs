using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTreasureItem : PassiveItem
{
    private static string name = "Test treasure item";
    private static string spritePath = "Sprites/Items/BigMushroom";
    public override string Name { get { return name; } }
    public override string SpritePath { get { return spritePath; } }
    public override void OnPickup(PlayerController player)
    {
        player.fireDelay *= 0.2f;
    }
}
