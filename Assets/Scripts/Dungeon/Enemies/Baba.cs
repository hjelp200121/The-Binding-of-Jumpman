using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baba : BasicEnemy
{
    public float minWanderTime = 0.5f;
    public float maxWanderTime = 1f;

    float currentWander;
    Vector2 wanderDirection;

    public override void Awake()
    {
        base.Awake();
        currentWander = Random.Range(minWanderTime, maxWanderTime);
        wanderDirection = ChooseWanderDirection();
    }

    public virtual void Update()
    {
        if ((currentWander -= Time.deltaTime) < 0f)
        {
            currentWander = Random.Range(minWanderTime, maxWanderTime);
            wanderDirection = ChooseWanderDirection();
            rb.velocity = wanderDirection * speed;
        }
    }

    public void FixedUpdate()
    {
        rb.velocity = rb.velocity.normalized * speed;
    }

    Vector2 ChooseWanderDirection()
    {
        float a = Random.Range(0f, 2 * Mathf.PI);
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }
}
