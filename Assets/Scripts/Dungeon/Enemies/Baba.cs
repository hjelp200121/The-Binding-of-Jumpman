using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baba : BasicEnemy
{
    public float wanderTime = 3f;

    float currentWander;
    Vector2 wanderDirection;

    public override void Awake() {
        base.Awake();
        currentWander = wanderTime;
        wanderDirection = ChooseWanderDirection();
    }

    public void Update() {
        if ((currentWander -= Time.deltaTime) < 0f) {
            currentWander = wanderTime;
            wanderDirection = ChooseWanderDirection();
        }
    }

    public void FixedUpdate() {
        rb.AddForce(wanderDirection * speed * Time.fixedDeltaTime);
    }

    Vector2 ChooseWanderDirection() {
        float a = Random.Range(0f, 2*Mathf.PI);
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }
}
