using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanessBone : PassiveItem
{
    public override void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        base.OnPickup(pedestal, player);
        //Lav epic bone foran spilleren
    }
}
