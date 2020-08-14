using System.Collections.Generic;

public class Basket : EquipableItem
{
    public Basket()
    {
        equippableSlot = EquipSlot.MainHand;
        stackable = false;
        tag = Tags.Basket;
        weight = 1;
        preFab = "Prefab/" + tag;
        itemEffects = new Dictionary<string, float>();
        itemEffects.Add(Tags.Berries, 1.1f);
        craftable = true;
        craftCost = new Dictionary<string, uint>();
        craftCost.Add(Tags.Wood, 4);
        crafted = false;
    }
    public Basket(float quality)
    {
        equippableSlot = EquipSlot.MainHand;
        stackable = false;
        tag = Tags.Basket;
        weight = 1;
        preFab = "Prefab/" + tag;
        itemEffects = new Dictionary<string, float>();
        itemEffects.Add(Tags.Berries, 1.1f);
        this.quality = quality;
        craftable = true;
        craftCost = new Dictionary<string, uint>();
        craftCost.Add(Tags.Wood, 4);
        crafted = true;
    }

    public override CraftedItem craft(float quality)
    {
        if (crafted)
            return this;

        return new Basket(quality);
    }
}
