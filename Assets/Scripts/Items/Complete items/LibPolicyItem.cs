using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibPolicyItem : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.hasLibPolicy = true;
    }
}
