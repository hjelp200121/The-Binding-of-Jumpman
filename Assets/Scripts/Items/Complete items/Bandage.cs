using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandage : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.maxHealth += 2;
        player.UpdateUI();
    }
}