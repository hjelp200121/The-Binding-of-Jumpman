using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BensGrill : PassiveItem
{
    public Projectile projectilePrefab1;
    public Projectile projectilePrefab2;

    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.hasBensGrill = true;
        player.Coalprefab = projectilePrefab1;
        player.BBQprefab = projectilePrefab2;
    }
}