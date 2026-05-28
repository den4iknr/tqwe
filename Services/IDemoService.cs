using Cs2DemoViewer.Maui.Models;

namespace Cs2DemoViewer.Maui.Services;

/// <summary>
/// Обёртка над Cs2DemoViewer.Core.Services.DemoParserService
/// </summary>
public interface IDemoService
{
    /// <summary>Парсит .dem файл и возвращает данные демки</summary>
    Task<DemoData> ParseDemoAsync(string filePath, CancellationToken ct = default);

    /// <summary>Список недавно открытых демок (хранится локально)</summary>
    IReadOnlyList<RecentDemo> GetRecentDemos();

    void AddToRecent(string filePath, string mapName, int roundCount);
}
