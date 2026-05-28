using Cs2DemoViewer.Core.Services;

namespace Cs2DemoViewer.Maui.Services;

public interface IMapService
{
    /// <summary>Возвращает MapCatalog из Core.dll для парсера</summary>
    MapCatalog GetCatalog();

    /// <summary>Путь к изображению карты (PNG/JPG из Resources/Images)</summary>
    string? GetMapImageName(string mapName);
}

public class MapService : IMapService
{
    // Путь к папке с картами — пользователь может положить PNG-файлы карт сюда
    // Имена файлов должны совпадать с именами карт CS2: de_dust2.png, de_mirage.png и т.д.
    private static readonly string MapsDir = Path.Combine(
        FileSystem.AppDataDirectory, "maps");

    public MapCatalog GetCatalog()
    {
        // MapCatalog читает JSON-конфиги из папки maps/
        // Если папки нет — создаём с встроенными конфигами
        EnsureMapsDirectory();
        return new MapCatalog(MapsDir);
    }

    public string? GetMapImageName(string mapName)
    {
        // Нормализуем имя карты (убираем префикс workshop и т.д.)
        var name = mapName.ToLower();
        if (name.Contains('/')) name = name.Split('/').Last();

        // Проверяем встроенные ресурсы (Resources/Images/maps/)
        var knownMaps = new[]
        {
            "de_dust2", "de_mirage", "de_inferno", "de_nuke",
            "de_overpass", "de_ancient", "de_anubis", "de_vertigo",
            "de_cache", "de_train", "de_cobblestone"
        };

        if (knownMaps.Contains(name))
            return $"map_{name}.png";  // файл должен быть в Resources/Images/

        return null;
    }

    private static void EnsureMapsDirectory()
    {
        if (!Directory.Exists(MapsDir))
            Directory.CreateDirectory(MapsDir);

        // Записываем встроенные JSON-конфиги для стандартных карт если их нет
        var configs = GetBuiltinMapConfigs();
        foreach (var (filename, content) in configs)
        {
            var path = Path.Combine(MapsDir, filename);
            if (!File.Exists(path))
                File.WriteAllText(path, content);
        }
    }

    /// <summary>
    /// Встроенные конфиги калибровки карт CS2.
    /// Значения scale/pos берутся из официальных radar-файлов CS2.
    /// </summary>
    private static IEnumerable<(string filename, string json)> GetBuiltinMapConfigs()
    {
        yield return ("de_dust2.json", """
        {
          "mapName": "de_dust2",
          "posX": -2476,
          "posY": 3239,
          "scale": 4.4,
          "size": 1024
        }
        """);

        yield return ("de_mirage.json", """
        {
          "mapName": "de_mirage",
          "posX": -3230,
          "posY": 1713,
          "scale": 5.0,
          "size": 1024
        }
        """);

        yield return ("de_inferno.json", """
        {
          "mapName": "de_inferno",
          "posX": -2087,
          "posY": 3870,
          "scale": 4.9,
          "size": 1024
        }
        """);

        yield return ("de_nuke.json", """
        {
          "mapName": "de_nuke",
          "posX": -3453,
          "posY": 2887,
          "scale": 7.0,
          "size": 1024
        }
        """);

        yield return ("de_overpass.json", """
        {
          "mapName": "de_overpass",
          "posX": -4831,
          "posY": 1781,
          "scale": 5.2,
          "size": 1024
        }
        """);

        yield return ("de_ancient.json", """
        {
          "mapName": "de_ancient",
          "posX": -2953,
          "posY": 2164,
          "scale": 5.0,
          "size": 1024
        }
        """);

        yield return ("de_anubis.json", """
        {
          "mapName": "de_anubis",
          "posX": -2796,
          "posY": 3328,
          "scale": 5.22,
          "size": 1024
        }
        """);

        yield return ("de_vertigo.json", """
        {
          "mapName": "de_vertigo",
          "posX": -3168,
          "posY": 1762,
          "scale": 4.0,
          "size": 1024
        }
        """);
    }
}
