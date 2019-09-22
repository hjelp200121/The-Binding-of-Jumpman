using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wasp : BasicEnemy
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Awake()
    {
        base.Awake();
    }

    public void FixedUpdate()
    {
        rb.velocity = (player.transform.position - transform.position).normalized * speed;
    }
}
