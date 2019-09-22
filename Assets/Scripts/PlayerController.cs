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
    private bool lookShoot;

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
        if (Input.GetKeyDown("space") && activeItem != null)
        {
            activeItem.Use(this);
            UpdateUI();
        }
        if (Input.GetKeyDown("e") && bombCount > 0)
        {
            bombCount--;
            ActiveBomb bomb = Instantiate<ActiveBomb>(bombPrefab);
            bomb.explosionDamage = bombDamage;
            bomb.explosionForce = bombForce;
            bomb.explosionRadius = bombRadius;
            bomb.fuseTime = bombFuseTime;
            bomb.transform.position = transform.position;
            currentRoom.contents.objects.Add(bomb);
            bomb.room = currentRoom;
            UpdateUI();
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
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

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
        /* Calculate the acceleration. */
        Vector2 velocityChange = new Vector2(horizontal, vertical);
        /* No resin allowed */
        if (velocityChange.sqrMagnitude > 1)
        {
            velocityChange = velocityChange.normalized;
        }
        velocityChange *= acceleration * Time.deltaTime;
        /* Only apply speed if not already going too fast. */
        if (rb.velocity.sqrMagnitude < speed*speed)
        {
            rb.velocity += velocityChange;
            /* If the added velocity was too much, dial the speed back. */
            if (rb.velocity.sqrMagnitude > speed*speed)
            {
                rb.velocity = rb.velocity.normalized * speed;
            }
        }
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

    public void Shoot()
    {
        /* Johan ændrer denne her method den er dum.
         * Shoot burde skyde, ikke andet.
         * Lav en anden method der tjekker om man kan skyde / om spilleren vil skyde.
         * Og så brug `Directions` enumeratoren, den gør ting nemmerer. */
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
            if (shootHorizontal != 0)
            {
                projectile.GetComponent<Rigidbody2D>().velocity =
                    new Vector2(shootHorizontal * shotSpeed, shootVertical * shotSpeed + Random.Range(-accuracy, accuracy) * shotSpeed);
            }
            else
            {
                projectile.GetComponent<Rigidbody2D>().velocity =
                    new Vector2(shootHorizontal * shotSpeed + Random.Range(-accuracy, accuracy) * shotSpeed, shootVertical * shotSpeed);
            }
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

    public void OnRoomClear()
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
