using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : DungeonEntity
{
    public abstract void OnPickup(PlayerController pickerUpper);

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D other = collision.collider;
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            OnPickup(player);
        }
    }
}
