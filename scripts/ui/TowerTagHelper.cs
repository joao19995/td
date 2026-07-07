using System.Collections.Generic;

public static class TowerTagHelper
{
    public static List<string> GetTags(TowerData towerData, string globalAuraLabel = "GLOBAL")
    {
        var tags = new List<string>();
        if (towerData.HasSplash) tags.Add("SPLASH");
        if (towerData.HasPoison) tags.Add("POISON");
        if (towerData.HasSlow) tags.Add("SLOW");
        if (towerData.HasAura) tags.Add("AURA");
        if (towerData.HasChain) tags.Add("CHAIN");
        if (towerData.HasCrit) tags.Add("CRIT");
        if (towerData.HasExecute) tags.Add("EXECUTE");
        if (towerData.HasGlobalAura) tags.Add(globalAuraLabel);
        return tags;
    }
}
