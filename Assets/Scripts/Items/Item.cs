using System;

[System.Serializable]
public abstract class Item
{
    public static bool taken = false;
    public abstract bool IsActive { get; }
    public abstract string Name { get; }
    public abstract string SpritePath { get; }

    public virtual void OnPickup(PlayerController player)
    {
        taken = true;
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
    public virtual bool IsUsable { get { return true; } }

    public virtual void OnUse(PlayerController player)
    {
        if (!IsUsable)
        {
            return;
        }
    }
}

[System.Serializable]
public abstract class DiscreteActiveItem : ActiveItem
{
    public static readonly int maxCharge;
    public int Charge { get; private set; }
    public override bool IsUsable { get { return Charge == maxCharge; } }

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
        if ((Charge += amount) > maxCharge)
        {
            Charge = maxCharge;
        }
    }
}