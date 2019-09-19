using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Pickup
{
    public override void Load () {
        gameObject.SetActive(true);
    }
    public override void UnLoad () {
        gameObject.SetActive(false);
    }

    public override void OnPickup (PlayerController pickerUpper) {
        pickerUpper.keys++;
        Destroy(gameObject);
    }
}
