namespace AfterlifeAdventures;

using System.Collections.Generic;
using System.IO;

public static class FileSystemUtils
{
    public static List<T> LoadAll<T>(string path) where T : class
    {
        using var dir = DirAccess.Open(path) ?? throw new Exception($"Failed to access path \"{path}\"");
        dir.ListDirBegin();

        string fileName = "initial";
        List<T> loaded = new();

        while (fileName != "")
        {
            try
            {
                LoadFile(loaded, Path.Combine(path, fileName = dir.GetNext()));
            }
            catch (Exception exception)
            {
                var filePath = Path.Combine(path, fileName);
                GD.PrintErr($"Failed to process \"{filePath}\": {exception}");
            }
        }

        dir.ListDirEnd();
        loaded = loaded.OfType<T>().ToList(); //* Filter out any null's
        return loaded;
    }

    private static void LoadFile<T>(List<T> loaded, string filePath) where T : class
    {
        if (!filePath.EndsWith(".tres")) return;

        loaded.Add(GD.Load<T>(filePath));
    }
}
