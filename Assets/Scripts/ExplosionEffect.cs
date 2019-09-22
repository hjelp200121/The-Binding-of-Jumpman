using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ExplosionEffect : MonoBehaviour
{
    public float radius = 2f;
    public float liveTime = 2f;
    Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
        Destroy(gameObject, liveTime);
    }
}
