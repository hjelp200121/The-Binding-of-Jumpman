using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegendaryHero : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.maxHealth += 2;
        player.health += 2;
        player.damage *= 1.25f;
        player.UpdateUI();
    }
}
