using System.Collections.Generic;

public class Hammer : EquipableItem
{
    public Hammer()
    {
        equippableSlot = EquipSlot.MainHand;
        stackable = false;
        tag = Tags.Hammer;
        weight = 1f;
        preFab = "Prefab/" + tag;
        itemEffects = new Dictionary<string, float>();
        itemEffects.Add(Tags.BuildSite, 1.1f);
        craftable = true;
        craftCost = new Dictionary<string, uint>();
        craftCost.Add(Tags.Wood, 4);
        crafted = false;
    }
    public Hammer(float quality)
    {
        equippableSlot = EquipSlot.MainHand;
        stackable = false;
        tag = Tags.Hammer;
        weight = 1f;
        preFab = "Prefab/" + tag;
        itemEffects = new Dictionary<string, float>();
        itemEffects.Add(Tags.BuildSite, 1.1f);
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

        return new Hammer(quality);
    }
}
