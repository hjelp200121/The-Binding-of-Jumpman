using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : DungeonEntity
{
    public int keys = 0;
    public Projectile projectilePrefab;
    private float lastFire;
    private bool lookShoot;

    // Actual ingame stats
    public int maxHealth;
    public int health;
    public float damage;
    public float fireDelay;
    public float shotTimer;
    public float shotSpeed;
    public float speed;
    public float invincibilityOnDamage;
    [HideInInspector]
    public float invincibilityTime;
    [HideInInspector]
    public bool invincible = false;

    // Initializations for different stuff
    SpriteRenderer spriteRenderer;
    Animator animator;

    IEnumerator blinkRoutine;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        health = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        blinkRoutine = Blink(0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if ((invincibilityTime -= Time.deltaTime) < 0f)
        {
            invincible = false;
            invincibilityTime = 0f;
            StopCoroutine(blinkRoutine);
            spriteRenderer.enabled = true;
        }
        else if (!invincible)
        {
            StartCoroutine(blinkRoutine);
            invincible = true;
        }
        Shoot();
        Move();
    }

    public void Die(DungeonObject source)
    {
        Debug.Log("Player was killed by " + source.name + ".");
    }

    public void Move()
    {
        float horizontal = Input.GetAxis("Horizontal") * speed;
        float vertical = Input.GetAxis("Vertical") * speed;

        if (Input.GetKey("a") == true && Input.GetKey("d") == true)
        {
            horizontal = 0;
        }
        if (Input.GetKey("w") == true && Input.GetKey("s") == true)
        {
            vertical = 0;
        }
        if (lookShoot == false)
        {
            if (Input.GetKey("a"))
            {
                animator.SetInteger("LookDirection", 3);
            }
            else if (Input.GetKey("d"))
            {
                animator.SetInteger("LookDirection", 1);
            }
            else if (Input.GetKey("w"))
            {
                animator.SetInteger("LookDirection", 0);
            }
            else
            {
                animator.SetInteger("LookDirection", 2);
            }
        }
        rb.velocity = new Vector2(horizontal, vertical);
    }

    public void Shoot()
    {
        float shootHorizontal = 0;
        float shootVertical = 0;
        if (Input.GetKey("left") && Time.time > lastFire + fireDelay)
        {
            shootHorizontal = -1;
            animator.SetInteger("LookDirection", 3);
            lookShoot = true;
        }
        else if (Input.GetKey("right") && Time.time > lastFire + fireDelay)
        {
            shootHorizontal = 1;
            animator.SetInteger("LookDirection", 1);
            lookShoot = true;
        }
        else if (Input.GetKey("up") && Time.time > lastFire + fireDelay)
        {
            shootVertical = 1;
            animator.SetInteger("LookDirection", 0);
            lookShoot = true;
        }
        else if (Input.GetKey("down") && Time.time > lastFire + fireDelay)
        {
            shootVertical = -1;
            animator.SetInteger("LookDirection", 2);
            lookShoot = true;
        }
        else if (Time.time > lastFire + fireDelay)
        {
            lookShoot = false;
        }

        if (shootHorizontal != 0 || shootVertical != 0)
        {
            lastFire = Time.time;
            Projectile projectile = Instantiate<Projectile>(projectilePrefab, transform.position, transform.rotation);
            projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(shootHorizontal * shotSpeed, shootVertical * shotSpeed);
            projectile.timer = shotTimer;
            projectile.damage = damage;
            projectile.fromPlayer = true;
            projectile.sender = this;
        }
    }

    public void TakeDamage(DungeonObject source)
    {
        TakeDamage(1, source);
    }

    public void TakeDamage(int amount, DungeonObject source)
    {
        if (invincible)
        {
            return;
        }
        if ((health -= amount) < 0)
        {
            Die(source);
        }
        invincibilityTime += invincibilityOnDamage;
    }

    public override void Load() { }
    public override void UnLoad() { }

    public IEnumerator Blink(float blinkTime)
    {
        while (true)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Pickup")
        {
            Pickup pickup = other.gameObject.GetComponent<Pickup>();
            pickup.OnPickup(this);
        }
    }

}
