using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossItem : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.speed *= 1.5f;
        player.acceleration *= 1.5f;
    }
}
