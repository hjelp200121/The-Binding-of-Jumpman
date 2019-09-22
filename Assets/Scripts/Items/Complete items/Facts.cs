using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Facts : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.maxHealth += 4;
        player.health += 4;
        player.UpdateUI();
    }
}