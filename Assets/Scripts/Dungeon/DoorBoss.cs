using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBoss : Door
{
    public override void BlowUp(DungeonObject source, float damage)
    {
        /* Boss doors cannot be blown up. */
    }
}
