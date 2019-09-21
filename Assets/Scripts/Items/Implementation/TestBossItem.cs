using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossItem : PassiveItem
{
    private static string name = "Test boss item";
    private static string spritePath = "Sprites/Items/0";
    public override string Name { get { return name; } }
    public override string SpritePath { get { return spritePath; } }
    public override void OnPickup(PlayerController player)
    {
        player.speed *= 1.5f;
    }
}
