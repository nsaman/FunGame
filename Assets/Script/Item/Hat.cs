using System.Collections.Generic;

public class Hat : EquipableItem
{
    public Hat()
    {
        equippableSlot = EquipSlot.Head;
        stackable = false;
        tag = Tags.Hat;
        weight = 1;
        preFab = "Prefab/" + tag;
        itemEffects = new Dictionary<string, float>();
        craftable = true;
        craftCost = new Dictionary<string, uint>();
        craftCost.Add(Tags.Wood, 5);
    }
}
