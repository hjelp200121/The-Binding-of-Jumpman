using System;

[System.Serializable]
public abstract class Item
{
    public static bool taken = false;
    public abstract bool IsActive { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string SpritePath { get; }

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
    public abstract int MaxCharge { get; }
    public int Charge { get; private set; }

    public override bool Use(PlayerController player)
    {
        if (Charge == MaxCharge)
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
        this.Charge = 0;
    }

    public void ChargeItem()
    {
        ChargeItem(1);
    }

    public void ChargeItem(int amount)
    {
        if ((Charge += amount) > MaxCharge)
        {
            Charge = MaxCharge;
        }
    }
}