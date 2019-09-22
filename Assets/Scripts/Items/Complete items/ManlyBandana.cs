using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManlyBandana : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.damage *= 1.25f;
        player.speed *= 1.25f;
        player.acceleration *= 1.25f;
    }
}