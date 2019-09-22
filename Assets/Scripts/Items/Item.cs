using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Item : MonoBehaviour
{
    public static bool taken = false;
    public string itemName;
    public string description;
    public abstract bool IsActive { get; }

    public virtual void OnPickup(ItemPedestal pedestal, PlayerController player)
    {
        taken = true;
        player.PickUpItem(pedestal, this);
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

    public void Awake()
    {
        this.Charge = maxCharge;
    }
}