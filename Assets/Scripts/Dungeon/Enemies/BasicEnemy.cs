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
}
