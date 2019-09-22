using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Pickup
{
    public override void Load () {
        gameObject.SetActive(true);
    }
    public override void UnLoad () {
        gameObject.SetActive(false);
    }

    public override void OnPickup (PlayerController pickerUpper) {
        pickerUpper.bombCount++;
        pickerUpper.UpdateUI();
        room.contents.objectRemoveQueue.Enqueue(this);
        Destroy(gameObject);
    }
}
