public static class Tags
{
    public static string Berries { get { return "Berries"; } }
    public static string BuildSite { get { return "BuildSite"; } }
    public static string Food { get { return "food"; } }
    public static string Ground { get { return "Ground"; } }
    public static string House { get { return "House"; } }
    public static string Npc { get { return "Npc"; } }
    public static string Teams { get { return "Teams"; } }
    public static string TownCenter { get { return "TownCenter"; } }
    public static string TownCenterDropOff { get { return "TownCenterDropOff"; } }
    public static string Wood { get { return "Wood"; } }
    public static string[] Teamed = new string[] { Tags.Npc, Tags.TownCenter, BuildSite, House };
    public static string[] Resources = new string[] { Tags.Wood, Tags.Food };
}
