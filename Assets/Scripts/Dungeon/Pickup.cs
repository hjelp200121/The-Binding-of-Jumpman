using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : DungeonEntity
{
    public abstract void OnPickup(PlayerController pickerUpper);
}
