using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemmieArmor : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.maxHealth += 2;
        player.health += 2;
        player.damage *= 1.25f;
        player.fireDelay *= 0.8f;
        player.shotTimer *= 1.25f;
        player.shotSpeed *= 1.25f;
        player.accuracy *= 0.8f;
        player.speed *= 1.25f;
        player.acceleration *= 1.25f;
        player.invincibilityOnDamage *= 1.25f;
        player.UpdateUI();
    }
}