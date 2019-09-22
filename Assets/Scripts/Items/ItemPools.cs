using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemPoolNames
{
    TREASURE, BOSS
}

public class ItemPools : MonoBehaviour
{
    public static ItemPools instance = null;

    [NamedArray(typeof(ItemPoolNames))]
    public ItemPool[] itemPools;

    void Awake ()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

[System.Serializable]
public class ItemPool : System.Object
{
    public List<Item> items;

    public Item GetRandomItem()
    {
        int index = UnityEngine.Random.Range(0, items.Count);
        Item item = items[index];
        items.RemoveAt(index);
        return item;
    }
}