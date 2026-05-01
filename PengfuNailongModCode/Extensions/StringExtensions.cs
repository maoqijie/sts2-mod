using Godot;
using PengfuNailongMod.PengfuNailongModCode;

namespace PengfuNailongMod.PengfuNailongModCode.Extensions;

public static class StringExtensions
{
    public static string ImagePath(this string path)
    {
        return ResourcePath(MainFile.ResPath, "images", path);
    }

    public static string CardImagePath(this string path)
    {
        path = ResourcePath(MainFile.ResPath, "images", "card_portraits", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find card image path: " + path);
        return ResourcePath(MainFile.ResPath, "images", "card_portraits", "card.png");
    }

    public static string BigCardImagePath(this string path)
    {
        path = ResourcePath(MainFile.ResPath, "images", "card_portraits", "big", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find big card image path: " + path);
        return ResourcePath(MainFile.ResPath, "images", "card_portraits", "big", "card.png");
    }

    public static string RelicImagePath(this string path)
    {
        path = ResourcePath(MainFile.ResPath, "images", "relics", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find relic image path: " + path);
        return ResourcePath(MainFile.ResPath, "images", "relics", "relic.png");
    }

    public static string BigRelicImagePath(this string path)
    {
        path = ResourcePath(MainFile.ResPath, "images", "relics", "big", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find big relic image path: " + path);
        return ResourcePath(MainFile.ResPath, "images", "relics", "big", "relic.png");
    }

    public static string CharacterUiPath(this string path)
    {
        return ResourcePath(MainFile.ResPath, "images", "charui", path);
    }

    public static string ModResourcePath(this string path)
    {
        return ResourcePath(MainFile.ResPath, path);
    }

    public static string PowerImagePath(this string path)
    {
        return ResourcePath(MainFile.ResPath, "images", "powers", path);
    }

    private static string ResourcePath(params string[] parts)
    {
        string path = parts[0].TrimEnd('/');
        for (int i = 1; i < parts.Length; i++)
        {
            path += "/" + parts[i].Trim('/');
        }

        return path;
    }
}
