using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : DungeonEntity, IExplodable
{
    public int keyCount = 1, bombCount = 1;
    public Projectile projectilePrefab;
    private float lastFire;

    public RectTransform healthPanel;
    public Text keyText, bombText;
    public Image activeItemImage;
    public Slider activeItemChargeBar;

    // Actual ingame stats
    public int maxHealth;
    public int health;
    public float damage;
    public float fireDelay;
    public float shotTimer;
    public float shotSpeed;
    public float accuracy;
    //public float knockback;
    public float speed;
    public float acceleration;
    public float friction;
    public ActiveBomb bombPrefab;
    public float bombDamage;
    public float bombRadius;
    public float bombForce;
    public float bombFuseTime;
    public float invincibilityOnDamage;

    public Image healthImagePrefab;
    public Sprite[] healthSprites;
    public Room currentRoom;
    public ActiveItem activeItem;
    public List<Item> items;

    [HideInInspector]
    public float invincibilityTime;
    [HideInInspector]
    public bool invincible = false;
    [HideInInspector]
    public Vector2 moveDirection;
    [HideInInspector]
    public Directions lookDirection;

    bool shooting = false;

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
        items = new List<Item>();

        healthPanel = GameObject.Find("Health Panel").GetComponent<RectTransform>();
        keyText = GameObject.Find("Keys Text").GetComponent<Text>();
        bombText = GameObject.Find("Bombs Text").GetComponent<Text>();
        activeItemImage = GameObject.Find("Active Item Image").GetComponent<Image>();
        activeItemChargeBar = GameObject.Find("Active Item Charge Bar").GetComponent<Slider>();

        UpdateUI();
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

        HandleInput();
        UpdateDirection();
        Move();

        if (shooting && Time.time > lastFire + fireDelay)
        {
            lastFire = Time.time;
            Shoot();
        }
    }

    public void Die(DungeonObject source)
    {
        Debug.Log("Player was killed by " + source.name + ".");
    }

    public void HandleInput()
    {
        /* Default look direction. */
        lookDirection = Directions.SOUTH;

        /* Movement. */
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");
        if (Input.GetKey("a") == true && Input.GetKey("d") == true)
        {
            moveDirection.x = 0;
        }
        if (Input.GetKey("w") == true && Input.GetKey("s") == true)
        {
            moveDirection.y = 0;
        }
        /* Update the `lookDirection` to match the movement. */
        if (moveDirection.x > 0)
        {
            lookDirection = Directions.EAST;
        }
        else if (moveDirection.x < 0)
        {
            lookDirection = Directions.WEST;
        }
        else if (moveDirection.y > 0)
        {
            lookDirection = Directions.NORTH;
        }

        /* Shooting. */
        if (Input.GetKey("left"))
        {
            lookDirection = Directions.WEST;
            shooting = true;
        }
        else if (Input.GetKey("right"))
        {
            lookDirection = Directions.EAST;
            shooting = true;
        }
        else if (Input.GetKey("up"))
        {
            lookDirection = Directions.NORTH;
            shooting = true;
        }
        else if (Input.GetKey("down"))
        {
            lookDirection = Directions.SOUTH;
            shooting = true;
        }
        else
        {
            shooting = false;
        }


        /* Active item usage. */
        if (Input.GetKeyDown("space") && activeItem != null)
        {
            activeItem.Use(this);
            UpdateUI();
        }

        /* Bomb placement. */
        if (Input.GetKeyDown("e") && bombCount > 0)
        {
            bombCount--;
            PlaceBomb();
            UpdateUI();
        }
    }

    public void UpdateDirection()
    {
        if (lookDirection == Directions.NORTH)
        {
            animator.SetInteger("LookDirection", 0);
        }
        else if (lookDirection == Directions.EAST)
        {
            animator.SetInteger("LookDirection", 1);
        }
        else if (lookDirection == Directions.SOUTH)
        {
            animator.SetInteger("LookDirection", 2);
        }
        else
        {
            animator.SetInteger("LookDirection", 3);
        }
    }

    public void Move()
    {
        /* Calculate the acceleration. */
        Vector2 velocityChange = moveDirection;
        if (velocityChange == Vector2.zero)
        {
            rb.velocity -= rb.velocity * friction * Time.deltaTime;
        }
        else
        {
            /* No resin allowed */
            velocityChange = velocityChange.normalized;
            velocityChange *= acceleration * Time.deltaTime;
            /* Only apply speed if not already going too fast. */
            if ((rb.velocity + velocityChange).sqrMagnitude < speed * speed)
            {
                rb.velocity += velocityChange;
            }
        }   
    }

    public void Shoot()
    {
        Projectile projectile = Instantiate<Projectile>(projectilePrefab, transform.position, transform.rotation);

        Vector2 projectileVel = lookDirection.ToVector() * shotSpeed;
        projectileVel += rb.velocity * 0.5f;
        projectileVel.x += Random.Range(-accuracy, accuracy);
        projectileVel.y += Random.Range(-accuracy, accuracy);
        projectile.rb.velocity = projectileVel;

        projectile.timer = shotTimer;
        projectile.damage = damage;
        projectile.fromPlayer = true;
        projectile.sender = this;

        projectile.room = currentRoom;
        currentRoom.contents.objectAddQueue.Enqueue(projectile);
    }

    public void PlaceBomb()
    {
        ActiveBomb bomb = Instantiate<ActiveBomb>(bombPrefab);
        bomb.explosionDamage = bombDamage;
        bomb.explosionForce = bombForce;
        bomb.explosionRadius = bombRadius;
        bomb.fuseTime = bombFuseTime;
        bomb.transform.position = transform.position;
        currentRoom.contents.objects.Add(bomb);
        bomb.room = currentRoom;
    }

    public void UpdateUI()
    {
        /* Update health. */
        if (healthPanel != null)
        {
            Image[] _hpImages = healthPanel.GetComponentsInChildren<Image>();
            List<Image> hpImages = new List<Image>(_hpImages);
            hpImages.RemoveAt(0);
            if (hpImages.Count > Mathf.Ceil(maxHealth / 2f))
            {
                int tooMany = hpImages.Count - (int)Mathf.Ceil(maxHealth / 2f);
                for (int i = 0; i < tooMany; i++)
                {
                    int j = hpImages.Count - i - 1;
                    hpImages.RemoveAt(j);
                    Destroy(hpImages[j].gameObject);
                }
            }
            else if (hpImages.Count < Mathf.Ceil(maxHealth / 2f))
            {
                int tooFew = (int)Mathf.Ceil(maxHealth / 2f) - hpImages.Count;
                for (int i = 0; i < tooFew; i++)
                {
                    Image hpImage = Instantiate<Image>(healthImagePrefab, healthPanel);
                    hpImage.rectTransform.Translate(
                        Vector2.right * (hpImages.Count + i) * hpImage.rectTransform.rect.width / 2);
                    hpImages.Add(hpImage);
                }
            }
            int index = 0;
            for (; index < health / 2; index++)
            {
                Image hpImage = hpImages[index];
                hpImage.sprite = healthSprites[2];
            }
            if (health % 2 == 1)
            {
                Image hpImage = hpImages[index++];
                hpImage.sprite = healthSprites[1];
            }
            for (; index < maxHealth / 2; index++)
            {
                Image hpImage = hpImages[index];
                hpImage.sprite = healthSprites[0];
            }
        }
        /* Update keys. */
        if (keyText != null)
        {
            keyText.text = keyCount.ToString("00");
        }
        /* Update bombs. */
        if (bombText != null)
        {
            bombText.text = bombCount.ToString("00");
        }
        /* Update active item. */
        if (activeItemImage != null && activeItemChargeBar != null)
        {
            if (activeItem == null)
            {
                activeItemImage.gameObject.SetActive(false);
                activeItemChargeBar.gameObject.SetActive(false);
            }
            else
            {
                activeItemImage.gameObject.SetActive(true);

                activeItemImage.sprite = activeItem.GetComponent<SpriteRenderer>().sprite;
                if (activeItem is DiscreteActiveItem)
                {
                    DiscreteActiveItem dActiveItem = activeItem as DiscreteActiveItem;
                    activeItemChargeBar.gameObject.SetActive(true);
                    activeItemChargeBar.maxValue = dActiveItem.maxCharge;
                    activeItemChargeBar.wholeNumbers = true;
                    activeItemChargeBar.value = dActiveItem.Charge;
                }
                else
                {
                    activeItemChargeBar.gameObject.SetActive(false);
                }
            }
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
        if ((health -= amount) <= 0)
        {
            health = 0;
            Die(source);
        }
        invincibilityTime += invincibilityOnDamage;
        UpdateUI();
    }

    public void Heal(int amount)
    {
        if ((health += amount) > maxHealth)
        {
            health = maxHealth;
        }
        UpdateUI();
    }

    public void BlowUp(DungeonObject source, float damage)
    {
        if (damage > 0f)
        {
            TakeDamage(2, source);
        }
    }

    public override void OnRoomBeaten()
    {
        if (activeItem is DiscreteActiveItem)
        {
            (activeItem as DiscreteActiveItem).ChargeItem();
            UpdateUI();
        }
    }

    public void PickUpItem(ItemPedestal pedestal, Item item)
    {
        if (item.IsActive)
        {
            pedestal.item = activeItem;
            activeItem = item as ActiveItem;
            if (pedestal.item != null)
            {
                pedestal.item.transform.SetParent(pedestal.itemParent, false);
                pedestal.item.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        else
        {
            items.Add(item);
            pedestal.item = null;
        }
        item.transform.SetParent(transform, false);
        item.transform.position = transform.position;
        item.transform.localScale = Vector3.one;
        item.GetComponent<SpriteRenderer>().enabled = false;
        UpdateUI();
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
