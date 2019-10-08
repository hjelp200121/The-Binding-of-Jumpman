using System;
using UnityEngine;

public class ItemMetaData
{
    public bool seen = false;
    public bool pickedUp = false;
}

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Item : MonoBehaviour
{
    public string itemName;
    public string description;
    public ItemMetaData metaData = null;
    public abstract bool IsActive { get; }

    public virtual void Start () {
        metaData.seen = true;
    }

    public virtual void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        metaData.pickedUp = true;
        player.PickUpItem(pedestal, this);
    }

    public static Item InstantiateItem(Item prefab) {
        Item item = Instantiate<Item>(prefab);
        item.metaData = prefab.metaData;
        return item;
    }

    public static Item InstantiateItem(Item prefab, Transform parent) {
        Item item = Instantiate<Item>(prefab, parent);
        item.metaData = prefab.metaData;
        return item;
    }
}

[System.Serializable]
public abstract class PassiveItem : Item
{
    public sealed override bool IsActive { get { return false; } }
}

[System.Serializable]
public abstract class ActiveItem : Item
{
    public sealed override bool IsActive { get { return true; } }
    public abstract bool Use(PlayerController player);
}

[System.Serializable]
public abstract class DiscreteActiveItem : ActiveItem
{
    public int maxCharge;

    public int Charge { get; private set; }

    public override bool Use(PlayerController player)
    {
        if (Charge == maxCharge)
        {
            Charge = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public DiscreteActiveItem()
    {
        this.Charge = maxCharge;
    }

    public void ChargeItem()
    {
        ChargeItem(1);
    }

    public void ChargeItem(int amount)
    {
        if ((Charge += amount) > maxCharge)
        {
            Charge = maxCharge;
        }
    }

    public override void Start()
    {
        base.Start();
        this.Charge = maxCharge;
    }
}