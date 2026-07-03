using System.Collections.Generic;

public static class SynergyPreviewHelper
{
    public static List<SynergyData> GetPreviewSynergies(List<string> towerIds)
    {
        var synergies = new List<SynergyData>();
        var all = ResourceLoaderHelper.LoadFromDir<SynergyData>("res://resources/synergy_data/");

        foreach (var synergy in all)
        {
            if (SaveManager.Instance != null && !SaveManager.Instance.IsDiscovered("synergy_" + synergy.Id))
                continue;

            bool allRequired = true;
            foreach (var reqId in synergy.RequiredTowerIds)
            {
                if (!towerIds.Contains(reqId))
                {
                    allRequired = false;
                    break;
                }
            }

            if (allRequired && towerIds.Count >= synergy.MinTowerCount)
                synergies.Add(synergy);
        }

        return synergies;
    }
}
