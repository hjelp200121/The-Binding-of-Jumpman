using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPedestal : DungeonEntity
{
    public ItemPoolNames poolNames;
    public Transform itemParent;
    private ItemPool pool;
    public Item item;

    private bool loadedBefore;
    Vector2 origPosition;

    public void Start()
    {
        loadedBefore = false;
    }

    void Update()
    {
        rb.velocity = (origPosition - (Vector2)transform.localPosition) * 10f;
        if (item != null)
        {
            itemParent.Translate(Vector2.up * Mathf.Sin(Time.time) * Time.deltaTime * 0.1f);
            itemParent.localScale = itemParent.localScale +
                                                Vector3.up * Mathf.Sin(Time.time * 0.9f) * Time.deltaTime * 0.05f;
        }
    }

    public override void Load()
    {
        if (!loadedBefore)
        {
            /* If it is the first time this pedestal has been loaded,
             * Set the original position & choose a random item. */
            loadedBefore = true;
            origPosition = transform.localPosition;

            if (ItemPools.instance != null)
            {
                pool = ItemPools.instance.itemPools[(int)poolNames];
                item = Instantiate<Item>(pool.GetRandomItem(), itemParent);
            }
        }
        if (room.cleared)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public override void UnLoad()
    {
        gameObject.SetActive(false);
    }

    public override void OnRoomClear()
    {
        gameObject.SetActive(true);
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
