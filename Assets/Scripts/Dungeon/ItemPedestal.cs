using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPedestal : DungeonEntity
{
    public ItemPoolNames poolNames;
    public SpriteRenderer itemRenderer;
    private ItemPool pool;
    public Item item;

    private bool loadedBefore = false;
    private Vector2 origPosition;

    public override void Awake()
    {
        base.Awake();
        if (!ItemPools.Intitialized)
        {
            ItemPools.Initialize();
        }
        pool = ItemPools.itemPools[poolNames];
    }

    void Update()
    {
        rb.velocity = (origPosition - (Vector2)transform.localPosition) * 10f;
        itemRenderer.transform.Translate(Vector2.up * Mathf.Sin(Time.time) * Time.deltaTime * 0.1f);
        itemRenderer.transform.localScale = itemRenderer.transform.localScale +
                                            Vector3.up * Mathf.Sin(Time.time * 0.9f) * Time.deltaTime * 0.05f;
    }

    public override void Load()
    {
        if (!loadedBefore)
        {
            /* If it is the first time this pedestal has been loaded,
             * Set the original position & choose a random item. */
            origPosition = transform.localPosition;
            item = pool.GetRandomItem();
        }
        loadedBefore = true;
        UpdateSpriteRenderer();
        gameObject.SetActive(true);
    }

    void UpdateSpriteRenderer()
    {
        if (item == null)
        {
            itemRenderer.sprite = null;
            return;
        }
        itemRenderer.sprite = Resources.Load<Sprite>(item.SpritePath);
        if (itemRenderer.sprite == null)
        {
            Debug.LogError("No sprite at '" + item.SpritePath + "'.");
        }
    }

    public override void UnLoad()
    {
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (item == null)
        {
            return;
        }
        Collider2D other = collision.collider;
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            item.OnPickup(this, player);
            UpdateSpriteRenderer();
        }
    }
}
