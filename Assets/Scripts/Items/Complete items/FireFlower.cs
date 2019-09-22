using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlower : PassiveItem
{
    public Projectile projectilePrefab;

    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.damage *= 1.3f;
        player.fireDelay *= 0.8f;
        player.hasFireFlower = true;
        player.Fireprefab = projectilePrefab;
    }
}