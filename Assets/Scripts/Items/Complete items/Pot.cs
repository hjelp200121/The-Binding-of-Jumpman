using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.maxHealth += 2;
        player.health += 2;
        player.fireDelay *= 0.8f;
        player.speed *= 0.8f;
        player.accuracy *= 2f;
        player.UpdateUI();
    }
}