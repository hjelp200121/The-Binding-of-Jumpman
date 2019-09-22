using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Enemy : DungeonEntity, IExplodable
{
    public float baseHealth;
    public float health;
    public PlayerController player;
    bool alive;
    public SpriteRenderer spriteRenderer;

    public override void Awake()
    {
        health = baseHealth;
        alive = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        base.Awake();
    }

    public virtual void Die() {
        if (alive)
        {
            alive = false;
            Delete();
            room.UpdateCleared();
        }
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Delete()
    {
        room.contents.enemyCount--;
        room.contents.objectRemoveQueue.Enqueue(this);
        Destroy(gameObject);
    }

    public void BlowUp(DungeonObject source, float damage)
    {
        TakeDamage(damage);
    }

    public override void Load()
    {
        player = room.player;
    }

    public override void UnLoad()
    {
        Delete();
    }
}
