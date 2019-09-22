using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : DungeonEntity
{
    public float damage;

    [HideInInspector]
    public bool fromPlayer;
    [HideInInspector]
    public float timer;
    [HideInInspector]
    public DungeonObject sender;

    bool deleted;

    public override void Awake()
    {
        base.Awake();
        deleted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!deleted)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Delete();
            }

        }
    }

    public virtual void Delete()
    {
        if (!deleted)
        {
            deleted = true;
            room.contents.objectRemoveQueue.Enqueue(this);
            Destroy(gameObject);
        }
    }

    public override void Load() { }
    public override void UnLoad()
    {
        Delete();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy" || other.tag == "Boss")
        {
            if (fromPlayer)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                enemy.TakeDamage(damage);
                Delete();
            }
        }
        else if (other.tag == "Player")
        {
            if (!fromPlayer)
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.TakeDamage(sender);
                Delete();
            }
        }
        else if (other.tag == "Active Bomb")
        {
            other.GetComponent<Rigidbody2D>().AddForce(rb.velocity * 0.2f, ForceMode2D.Impulse);
            Delete();
        }
        else
        {
            if (!other.isTrigger)
            {
                Delete();
            }
        }
    }
}
