using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;

    [HideInInspector]
    public bool fromPlayer;
    [HideInInspector]
    public float timer;
    [HideInInspector]
    public DungeonObject sender;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy" || other.tag == "Boss")
        {
            if (fromPlayer)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else if (other.tag == "Player")
        {
            if (!fromPlayer)
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.TakeDamage(sender);
                Destroy(gameObject);
            }
        }
        else
        {
            if (!other.isTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}
