using Godot;
using System.Collections.Generic;

public static class ResourceLoaderHelper
{
    public static List<T> LoadFromDir<T>(string dirPath) where T : Resource
    {
        var items = new List<T>();
        var dir = DirAccess.Open(dirPath);
        if (dir == null) return items;

        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;
            var res = ResourceLoader.Load<Resource>(dirPath + file, "", ResourceLoader.CacheMode.Replace);
            if (res is T item)
                items.Add(item);
        }

        return items;
    }
}
