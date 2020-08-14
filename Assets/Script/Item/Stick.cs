using System.Collections.Generic;

public class Stick : EquipableItem
{
    public Stick()
    {
        equippableSlot = EquipSlot.MainHand;
        stackable = false;
        tag = Tags.Stick;
        weight = 1;
        preFab = "Prefab/" + tag;
        itemEffects = new Dictionary<string, float>();
        itemEffects.Add(Tags.Tree, 1.1f);
        craftable = true;
        craftCost = new Dictionary<string, uint>();
        craftCost.Add(Tags.Wood, 2);
        crafted = false;
    }
    public Stick(float quality)
    {
        equippableSlot = EquipSlot.MainHand;
        stackable = false;
        tag = Tags.Stick;
        weight = 1;
        preFab = "Prefab/" + tag;
        itemEffects = new Dictionary<string, float>();
        itemEffects.Add(Tags.Tree, 1.1f);
        this.quality = quality;
        craftable = true;
        craftCost = new Dictionary<string, uint>();
        craftCost.Add(Tags.Wood, 2);
        crafted = true;
    }

    public override CraftedItem craft(float quality)
    {
        if (crafted)
            return this;

        return new Stick(quality);
    }
}
