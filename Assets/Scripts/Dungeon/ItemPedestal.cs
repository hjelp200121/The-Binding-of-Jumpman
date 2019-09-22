using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPedestal : DungeonEntity
{
    public ItemPoolNames poolNames;
    public Transform itemParent;
    private ItemPool pool;
    public Item item;

    private bool loadedBefore = false;
    private Vector2 origPosition;

    public override void Awake()
    {
        base.Awake();
        pool = ItemPools.instance.itemPools[(int)poolNames];
    }

    void Update()
    {
        rb.velocity = (origPosition - (Vector2)transform.localPosition) * 10f;
        itemParent.Translate(Vector2.up * Mathf.Sin(Time.time) * Time.deltaTime * 0.1f);
        itemParent.localScale = itemParent.localScale +
                                            Vector3.up * Mathf.Sin(Time.time * 0.9f) * Time.deltaTime * 0.05f;
    }

    public override void Load()
    {
        if (!loadedBefore)
        {
            /* If it is the first time this pedestal has been loaded,
             * Set the original position & choose a random item. */
            origPosition = transform.localPosition;
            item = Instantiate<Item>(pool.GetRandomItem(), itemParent);
        }
        loadedBefore = true;
        gameObject.SetActive(true);
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
        }
    }
}
