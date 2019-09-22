using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.accuracy = 0;
        player.shotSpeed = 30;
    }
}