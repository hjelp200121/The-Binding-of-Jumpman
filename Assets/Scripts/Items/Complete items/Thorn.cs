using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorn : PassiveItem
{
    public Projectile projectilePrefab;

    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.hasThorn = true;
        player.Thornprefab = projectilePrefab;
        player.damage *= 1.25f;
    }
}