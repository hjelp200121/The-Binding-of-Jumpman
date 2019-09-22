using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTreasureItem : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.fireDelay *= 0.2f;
    }
}
