using System.Linq;

public static class Tags
{
    public static string Basket { get { return "Basket"; } }
    public static string Berries { get { return "Berries"; } }
    public static string BuildSite { get { return "BuildSite"; } }
    public static string Food { get { return "food"; } }
    public static string Ground { get { return "Ground"; } }
    public static string Hammer { get { return "Hammer"; } }
    public static string Hat { get { return "Hat"; } }
    public static string House { get { return "House"; } }
    public static string Npc { get { return "Npc"; } }
    public static string Stick { get { return "Stick"; } }
    public static string Teams { get { return "Teams"; } }
    public static string TownCenter { get { return "TownCenter"; } }
    public static string TownCenterDropOff { get { return "TownCenterDropOff"; } }
    public static string Wood { get { return "Wood"; } }

    public static string[] EquipableHeads = new string[] { Tags.Hat };
    public static string[] EquipableChests = new string[] {  };
    public static string[] EquipableMainHands = new string[] { Tags.Stick, Tags.Hammer, Tags.Basket };
    public static string[] EquipableOffHands = new string[] {  };
    public static string[] EquipableLegs = new string[] {  };
    public static string[] EquipableFeets = new string[] {  };
    public static string[] Teamed = new string[] { Tags.Npc, Tags.TownCenter, BuildSite, House };
    public static string[] Resources = new string[] { Tags.Wood, Tags.Food };
    public static string[] Equipables = EquipableHeads.Union(EquipableChests).Union(EquipableMainHands).Union(EquipableOffHands).Union(EquipableLegs).Union(EquipableFeets).ToArray();
}
