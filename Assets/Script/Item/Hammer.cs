using System.Collections.Generic;

public class Hammer : EquipableItem
{
    public Hammer()
    {
        equippableSlot = EquipSlot.MainHand;
        stackable = false;
        tag = Tags.Hammer;
        weight = 1;
        preFab = "Prefab/" + tag;
        itemEffects = new Dictionary<string, float>();
        itemEffects.Add(Tags.BuildSite, 1.1f);
        craftable = true;
        craftCost = new Dictionary<string, uint>();
        craftCost.Add(Tags.Wood, 4);
    }
}
