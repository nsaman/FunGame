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
        itemEffects.Add(Tags.Wood, 1.1f);
        craftable = true;
        craftCost = new Dictionary<string, uint>();
        craftCost.Add(Tags.Wood, 2);
    }
}
