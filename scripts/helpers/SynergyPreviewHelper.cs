using System.Collections.Generic;

public static class SynergyPreviewHelper
{
    public static List<SynergyData> GetPreviewSynergies(List<string> towerIds)
    {
        var all = ResourceLoaderHelper.LoadFromDir<SynergyData>("res://resources/synergy_data/");
        return GetPreviewSynergies(towerIds, all);
    }

    public static List<SynergyData> GetPreviewSynergies(List<string> towerIds, IEnumerable<SynergyData> allSynergies)
    {
        var synergies = new List<SynergyData>();

        foreach (var synergy in allSynergies)
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
