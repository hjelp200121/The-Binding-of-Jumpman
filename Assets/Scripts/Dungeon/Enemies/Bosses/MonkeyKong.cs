using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MonkeyKong : Boss
{
    enum MonkeyKongActions
    {
        WALKING, SLAMMING, RUNNING, DAZED
    }
    public float walkSpeed;
    public float runForce;
    public float specialAttackCooldown;
    public float slamForce;
    public float slamRadius;
    public float slamPreDuration;
    public float slamPostDuration;
    public float collisionDazedDuration;
    public float slamDazedDuration;
    public float collsionForceModifier;
    public float collsionRadiusModifier;
    MonkeyKongActions action;
    Animator animator;

    float lastSpecialAttack;
    float dazedTime;
    public float slamPreTime;
    public float slamPostTime;

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (action == MonkeyKongActions.WALKING)
        {
            Vector2 walkDirection = (player.transform.position - transform.position).normalized;
            rb.velocity = walkDirection * walkSpeed;

            if (Time.time > lastSpecialAttack + specialAttackCooldown)
            {
                if (Random.Range(0, 2) == 1)
                {
                    action = MonkeyKongActions.RUNNING;
                    animator.SetTrigger("Running");
                }
                else
                {
                    action = MonkeyKongActions.SLAMMING;
                    animator.SetTrigger("Slamming");
                    slamPreTime = slamPostTime = 0f;
                }
                lastSpecialAttack = Time.time;
            }
        }
        else if (action == MonkeyKongActions.RUNNING)
        {
            RunAttack();
        }
        else if (action == MonkeyKongActions.SLAMMING)
        {
            slamPreTime += Time.deltaTime;
            if (slamPreTime > slamPreDuration)
            {
                if (slamPostTime == 0f)
                {
                    Debug.Log("slammin");
                    Slam();
                }
                slamPostTime += Time.deltaTime;
                if (slamPostTime > slamPostDuration)
                {
                    slamPreTime = 0f;
                    slamPostTime = 0f;
                    action = MonkeyKongActions.DAZED;
                    animator.SetTrigger("Dazed");
                }
            }
        }
        else
        {
            if ((dazedTime -= Time.deltaTime) < 0)
            {
                action = MonkeyKongActions.WALKING;
                animator.SetTrigger("Walking");
            }
        }
    }

    void RunAttack()
    {
        Vector2 forceDirection = (player.transform.position - transform.position).normalized;
        rb.AddForce(forceDirection * runForce * Time.deltaTime * (Time.time - lastSpecialAttack));
    }

    public void Slam()
    {
        Explosion.Explode(this, 20f,
            slamForce, transform.position,
            slamRadius);

        lastSpecialAttack = Time.time;
        rb.velocity = Vector2.zero;

    }

    public override void Load()
    {
        base.Load();
        lastSpecialAttack = Time.time;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D other = collision.collider;
        if (action == MonkeyKongActions.RUNNING)
        {
            /* If running, blow up everything it Monkey Kong's path. */
            var scripts = other.GetComponents<MonoBehaviour>();
            IExplodable[] interfaceScripts = (
                from a in scripts
                where a.GetType().GetInterfaces().Any(
                    k => k == typeof(IExplodable))
                select (IExplodable)a).ToArray();

            foreach (IExplodable explodable in interfaceScripts)
            {
                explodable.BlowUp(this, 20f);
            }

            if (other.tag == "Room Wall")
            {
                lastSpecialAttack = Time.time;
                rb.velocity = Vector2.zero;
                Explosion.Explode(this, 20f,
                    collsionForceModifier * rb.velocity.magnitude, transform.position,
                    collsionRadiusModifier * rb.velocity.magnitude);

                dazedTime = collisionDazedDuration;
                action = MonkeyKongActions.DAZED;
                animator.SetTrigger("Dazed");

            }
        }
        else
        {
            if (other.tag == "Player")
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.TakeDamage(this);
            }
        }
    }
}
