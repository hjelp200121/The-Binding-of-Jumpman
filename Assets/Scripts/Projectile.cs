﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float timer;

    // Start is called before the first frame update
    void Start(){
    }

    // Update is called once per frame
    void Update(){
        timer -= Time.deltaTime;
        Debug.Log(timer);
        if(timer <= 0) {
            Destroy(gameObject);
        }
    }
}