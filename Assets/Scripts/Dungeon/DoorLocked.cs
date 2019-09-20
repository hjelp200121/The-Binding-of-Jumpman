﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLocked : Door
{
    public bool locked = true;

    public override void Open()
    {
        if (!locked) {
            base.Open();
        } else {
            Close();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!locked) {
            return;
        }
        Collider2D other = collision.collider;
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            /* Take a key from the player, if they have one. */
            if (player.keys > 0)
            {
                player.keys--;
                locked = false;
                (connection as DoorLocked).locked = false;
                room.UpdateCleared();
            }
        }
    }
}
