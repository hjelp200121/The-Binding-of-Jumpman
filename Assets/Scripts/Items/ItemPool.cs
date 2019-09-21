using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemPoolNames
{
    TREASURE, BOSS
}

public class ItemPool
{
    private Type[] items;

    public ItemPool(Type[] items)
    {
        this.items = items;
    }

    public Item GetRandomItem()
    {
        int index = UnityEngine.Random.Range(0, items.Length);
        /* Get the item type at the random index and call its constructor. */
        Item item = items[index].GetConstructor(Type.EmptyTypes).Invoke(Type.EmptyTypes) as Item;
        return item;
    }
}

public static class ItemPools
{
    public static Dictionary<ItemPoolNames, ItemPool> itemPools;

    public static bool Intitialized { get; private set; }

    public static void Initialize()
    {
        if (Intitialized)
        {
            /* If the item pools have already been initialized, abort. */
            return;
        }
        Intitialized = true;

        itemPools = new Dictionary<ItemPoolNames, ItemPool>();
        /* Add the treasure item pool. */
        itemPools.Add(
            ItemPoolNames.TREASURE,
            new ItemPool(new Type[]{
                typeof(TestTreasureItem)
            })
        );
        /* Add the boss item pool. */
        itemPools.Add(
            ItemPoolNames.BOSS,
            new ItemPool(new Type[]{
                typeof(TestBossItem)
            })
        );
    }
}