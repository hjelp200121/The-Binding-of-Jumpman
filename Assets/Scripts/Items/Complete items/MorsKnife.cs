using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorsKnife : PassiveItem
{
    public Projectile projectilePrefab;

    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.hasMorsKnife = true;
        player.Knifeprefab = projectilePrefab;
    }
}