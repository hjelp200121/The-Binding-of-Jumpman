using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WornDagger : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.damage *= 2f;
        player.accuracy *= 3f;
    }
}
