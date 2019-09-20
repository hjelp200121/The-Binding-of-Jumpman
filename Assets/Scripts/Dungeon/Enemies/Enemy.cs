using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : DungeonEntity
{
    public SpriteRenderer renderer;
    public float baseHealth;
    float health;
    PlayerController player;

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
        Destroy(gameObject);
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
