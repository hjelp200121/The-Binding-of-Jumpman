using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    public float baseSpeed;

    [HideInInspector]
    public float speed;

    public override void Awake() {
        speed = baseSpeed;
        base.Awake();
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        Collider2D other = collision.collider;
        if (other.tag == "Player") {
            if (player.lawnPower) {
                health -= 10;
            } else {
                PlayerController player = other.GetComponent<PlayerController>();
                player.TakeDamage(this);
            }
        }
    }
}
