using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasPolicyItem : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.hasFasPolicy = true;
        player.temporaryDamage = player.damage;
    }
}
