﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandage : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        player.maxHealth += 2;
        player.health += 2;
        player.accuracy *= 0.5f;
        player.UpdateUI();
    }
}