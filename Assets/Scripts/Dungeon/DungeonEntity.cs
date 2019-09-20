using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class DungeonEntity : DungeonObject
{
    public Rigidbody2D rb;

    public bool flying = false;

    public virtual void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
    }
}
