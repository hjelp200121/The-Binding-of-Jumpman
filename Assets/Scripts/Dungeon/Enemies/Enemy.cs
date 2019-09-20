using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Enemy : DungeonEntity
{
    public float baseHealth;
    public float health;
    public PlayerController player;

    public override void Awake()
    {
        health = baseHealth;
        base.Awake();
    }

    public virtual void Die() {
        Delete();
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
        room.contents.objectRemoveQueue.Enqueue(this);
        Destroy(gameObject);
    }

    public override void Load()
    {
        player = room.player;
        Debug.Log("enemy " + player);
    }

    public override void UnLoad()
    {
        Delete();
    }
}
