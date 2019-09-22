using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiberalArtsDegree : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.maxHealth += 2;
        player.speed *= 1.5f;
        player.speed *= 1.5f;
        player.UpdateUI();
    }
}