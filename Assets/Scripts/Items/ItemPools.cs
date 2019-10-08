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
    public Dictionary<Type, ItemMetaData> itemMetaData;

    [NamedArray(typeof(ItemPoolNames))]
    public ItemPool[] itemPools;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        itemMetaData = new Dictionary<Type, ItemMetaData>();
        foreach (ItemPool pool in itemPools)
        {
            foreach (Item item in pool.items)
            {
                Type key = item.GetType();
                if (!itemMetaData.ContainsKey(key))
                {
                    ItemMetaData metaData = new ItemMetaData();
                    itemMetaData.Add(key, metaData);
                }
                item.metaData = itemMetaData[key];
            }
        }
    }
}

[System.Serializable]
public class ItemPool : System.Object
{
    public List<Item> items;

    public Item GetRandomItem()
    {
        if (items.Count == 0)
        {
            return null;
        }
        List<Item> eligebelItems = items.FindAll(
            i => !ItemPools.instance.itemMetaData[i.GetType()].seen);
        if (eligebelItems.Count == 0) {
            return null;
        }
        int index = UnityEngine.Random.Range(0, eligebelItems.Count);
        Item item = eligebelItems[index];
        return item;
    }
}