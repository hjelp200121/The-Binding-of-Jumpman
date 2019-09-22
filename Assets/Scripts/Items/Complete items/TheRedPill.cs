using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheRedPill : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.damage *= 1.5f;
    }
}