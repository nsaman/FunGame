using System.Collections.Generic;

public abstract class EquipableItem : Item {

    protected EquipSlot equippableSlot;
    protected Dictionary<string, float> itemEffects;

    public EquipSlot EquippableSlot
    {
        get
        {
            return equippableSlot;
        }
    }

    public float getEffectivenessByTag(string tag)
    {
        if (itemEffects.ContainsKey(tag))
            return (itemEffects[tag]);
        else
            return 0;
    }

    public enum EquipSlot
    {
        Head,
        Chest,
        MainHand,
        OffHand,
        Legs,
        Feet
    }

    public static HashSet<EquipableItem> getItemsByEquipSlot(EquipSlot equipSlot)
    {
        HashSet<EquipableItem> items = new HashSet<EquipableItem>();
        ICollection<string> tagsToAdd = null;

        if (equipSlot == EquipSlot.Head)
            tagsToAdd = Tags.EquipableHeads;
        else if (equipSlot == EquipSlot.Chest)
            tagsToAdd = Tags.EquipableChests;
        else if (equipSlot == EquipSlot.MainHand)
            tagsToAdd = Tags.EquipableMainHands;
        else if (equipSlot == EquipSlot.OffHand)
            tagsToAdd = Tags.EquipableOffHands;
        else if (equipSlot == EquipSlot.Legs)
            tagsToAdd = Tags.EquipableLegs;
        else if (equipSlot == EquipSlot.Feet)
            tagsToAdd = Tags.EquipableFeets;

        if (tagsToAdd == null)
            return items;

        foreach (string tag in tagsToAdd)
            items.Add((EquipableItem)Items.createItemByTag(tag));

        return items;
    }
}
