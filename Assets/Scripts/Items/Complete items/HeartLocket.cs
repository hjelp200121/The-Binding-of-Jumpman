using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartLocket : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.hasHeartLocket = true;
        player.heartLocketActive = true;
    }
}