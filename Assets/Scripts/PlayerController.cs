using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : DungeonEntity, IExplodable
{
    public int keyCount = 1, bombCount = 1;
    public Projectile projectilePrefab;
    public Projectile projectileUsed;
    private int shottype;
    private float lastFire;

    public RectTransform healthPanel;
    public Text keyText, bombText;
    public Image activeItemImage;
    public Slider activeItemChargeBar;
    public Text itemNameText;
    public Text itemDescriptionText;

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

    public bool hasHeartLocket = false;
    public bool heartLocketActive = false;
    public bool hasLibPolicy = false;
    public float shieldTime = 0;
    public bool hasFasPolicy = false;
    public float temporaryDamage = 3.5f;
    public bool hasMorsKnife = false;
    public Projectile Knifeprefab;
    public bool hasThorn = false;
    public Projectile Thornprefab;
    public bool lawnPower = false;
    public float tempSpeed;
    public float tempAcceleration;
    public bool hasBensGrill = false;
    public Projectile Coalprefab;
    public Projectile BBQprefab;
    public bool hasFireFlower = false;
    public Projectile Fireprefab;

    // Initializations for different stuff
    SpriteRenderer spriteRenderer;
    Animator animator;

    IEnumerator blinkRoutine;

    Item lastItem;

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
        projectileUsed = projectilePrefab;
        tempSpeed = speed;
        tempAcceleration = acceleration;

        lastItem = null;

        UpdateUI();

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(healthPanel.transform.root);
    }

    // Update is called once per frame
    void Update()
    {
        if ((invincibilityTime -= Time.deltaTime) < 0f && shieldTime < 0)
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
        ClampPosition();

        if (shooting && Time.time > lastFire + fireDelay)
        {
            lastFire = Time.time;
            Shoot();
        }

        if (!lawnPower)
        {
            tempAcceleration = acceleration;
            tempSpeed = speed;
        }

        shieldTime -= Time.deltaTime;
        if (shieldTime < 0.1 && shieldTime > -0.1)
        {
            invincible = false;
            lawnPower = false;
            tempSpeed = speed;
            tempAcceleration = acceleration;
        }
    }

    public void Die(DungeonObject source)
    {
        Application.Quit();
#if UNITY_EDITOR
        if(EditorApplication.isPlaying) 
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown("escape"))
        {
            Debug.Log("Quit tid");
            Application.Quit();
#if UNITY_EDITOR
            if(EditorApplication.isPlaying) 
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif
        }
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
            velocityChange *= tempAcceleration * Time.deltaTime;
            /* Only apply speed if not already going too fast. */
            if ((rb.velocity + velocityChange).sqrMagnitude < tempSpeed * tempSpeed)
            {
                rb.velocity += velocityChange;
            }
        }
    }

    public void ClampPosition()
    {
        if (currentRoom != null)
        {
            Vector2 newPos = transform.position;
            if (newPos.x < currentRoom.transform.position.x - (Room.gridWidth + 1) / 2f ||
                newPos.x > currentRoom.transform.position.x + (Room.gridWidth + 1) / 2f)
            {
                newPos.x = Mathf.Clamp(newPos.x,
                            currentRoom.transform.position.x - (Room.gridWidth - 1) / 2f,
                            currentRoom.transform.position.x + (Room.gridWidth - 1) / 2f);
            }
            if (newPos.y < currentRoom.transform.position.y - (Room.gridHeight + 1) / 2f ||
                newPos.y > currentRoom.transform.position.y + (Room.gridHeight + 1) / 2f)
            {
                newPos.y = Mathf.Clamp(newPos.y,
                            currentRoom.transform.position.y - (Room.gridHeight - 1) / 2f,
                            currentRoom.transform.position.y + (Room.gridHeight - 1) / 2f);
            }

            transform.position = newPos;
        }
    }

    public void Shoot()
    {
        if (currentRoom == null)
        {
            return;
        }
        shottype = Random.Range(0, 101);
        if (hasMorsKnife)
        {
            if (shottype < 20)
            {
                projectileUsed = Knifeprefab;

            }
            else
            {
                projectileUsed = projectilePrefab;
            }
        }
        if (hasThorn)
        {
            if (shottype > 20 && shottype < 50)
            {
                projectileUsed = Thornprefab;
            }
            else if (shottype > 50)
            {
                projectileUsed = projectilePrefab;
            }
        }
        if (hasBensGrill)
        {
            if (shottype > 50 && shottype < 60)
            {
                projectileUsed = Coalprefab;
            }
            else if (shottype > 60 && shottype < 70)
            {
                projectileUsed = BBQprefab;
            }
            else if (shottype > 70)
            {
                projectileUsed = projectilePrefab;
            }
        }
        if (hasFireFlower)
        {
            if (projectileUsed == projectilePrefab)
            {
                projectileUsed = Fireprefab;
            }
        }
        Projectile projectile = Instantiate<Projectile>(projectileUsed, transform.position, transform.rotation);

        Vector2 projectileVel = lookDirection.ToVector() * shotSpeed;
        projectileVel += rb.velocity * 0.5f;
        projectileVel.x += Random.Range(-accuracy, accuracy);
        projectileVel.y += Random.Range(-accuracy, accuracy);
        projectile.rb.velocity = projectileVel;

        projectile.timer = shotTimer;
        if (hasFasPolicy)
        {
            projectile.damage = temporaryDamage;
        }
        else
        {
            projectile.damage = damage;
        }

        if (projectileUsed == Knifeprefab)
        {
            projectile.damage += damage * 2;
        }
        else if (projectileUsed == Thornprefab)
        {
            projectile.damage += damage / 2;
            projectile.timer += shotTimer / 2;
        }
        else if (projectileUsed == Coalprefab)
        {
            projectile.timer -= shotTimer / 2;
            projectile.rb.velocity *= 0.8f;
        }
        else if (projectileUsed == BBQprefab)
        {
            projectile.damage *= 7;
            projectile.timer += shotTimer / 2;
            projectile.rb.velocity *= 1.25f;
        }

        projectile.fromPlayer = true;
        projectile.sender = this;

        projectile.room = currentRoom;
        currentRoom.contents.objectAddQueue.Enqueue(projectile);
        if (hasFasPolicy)
        {
            temporaryDamage *= 1.01f;
        }
    }

    public void PlaceBomb()
    {
        if (currentRoom == null)
        {
            return;
        }
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
                int newMax = (int)Mathf.Ceil(maxHealth / 2f);
                Debug.Log(newMax);
                Debug.Log(hpImages.Count - 1);
                for (int i = hpImages.Count - 1; newMax <= i; i--)
                {
                    Debug.Log("i: " + i);
                    Destroy(hpImages[i].gameObject);
                    hpImages.RemoveAt(i);
                }
            }
            else if (hpImages.Count < Mathf.Ceil(maxHealth / 2f))
            {
                int newMax = (int)Mathf.Ceil(maxHealth / 2f);
                for (int i = hpImages.Count; i < newMax; i++)
                {
                    Image hpImage = Instantiate<Image>(healthImagePrefab, healthPanel);
                    hpImage.rectTransform.Translate(
                        Vector2.right * i * hpImage.rectTransform.rect.width);
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
        if (heartLocketActive)
        {
            heartLocketActive = false;
            invincibilityTime += invincibilityOnDamage * 2;
            return;
        }
        if ((health -= amount) <= 0)
        {
            health = 0;
            Die(source);
        }
        invincibilityTime += invincibilityOnDamage;
        if (hasLibPolicy && health == 1)
        {
            invincible = true;
            shieldTime = 5;
        }
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
        if (hasHeartLocket)
        {
            heartLocketActive = true;
        }
        temporaryDamage = damage;
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

        Debug.Log("hej: " + item.itemName);
        lastItem = item;
        StopCoroutine("ShowItemInfo");
        StartCoroutine("ShowItemInfo");

        UpdateUI();
    }

    public override void Load() { }
    public override void UnLoad() { }

    public IEnumerator ShowItemInfo()
    {
        Debug.Log("------sa");
        if (lastItem != null)
        {
            itemNameText.text = lastItem.itemName;
            itemDescriptionText.text = lastItem.description;
        }
        float ft = 0;
        Color c;
        for (ft = 0; ft <= 1f; ft += 0.1f)
        {
            c = itemNameText.color;
            c.a = ft;
            itemNameText.color = c;
            itemDescriptionText.color = c;
            yield return null;
        }
        ft = 1;
        c = itemNameText.color;
        c.a = ft;
        itemNameText.color = c;
        itemDescriptionText.color = c;

        yield return new WaitForSeconds(3f);

        for (ft = 1; ft >= 0f; ft -= 0.1f)
        {
            c = itemNameText.color;
            c.a = ft;
            itemNameText.color = c;
            itemDescriptionText.color = c;
            yield return null;
        }
        ft = 0;
        c = itemNameText.color;
        c.a = ft;
        itemNameText.color = c;
        itemDescriptionText.color = c;
    }

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

    public void ActivateLawnmower()
    {
        invincible = true;
        tempSpeed = 30;
        lawnPower = true;
        shieldTime = 2;
        tempAcceleration = 1000;
    }
}
