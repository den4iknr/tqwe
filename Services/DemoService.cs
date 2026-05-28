using Cs2DemoViewer.Core.Services;
using Cs2DemoViewer.Maui.Models;

namespace Cs2DemoViewer.Maui.Services;

public class DemoService : IDemoService
{
    private readonly IMapService _mapService;
    private readonly List<RecentDemo> _recent = new();
    private const string RecentKey = "recent_demos";

    public DemoService(IMapService mapService)
    {
        _mapService = mapService;
        LoadRecent();
    }

    /// <summary>
    /// Парсит .dem файл используя оригинальный DemoParserService из Core.dll
    /// </summary>
    public async Task<DemoData> ParseDemoAsync(string filePath, CancellationToken ct = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не найден: {filePath}");

        var mapCatalog = _mapService.GetCatalog();
        var parser = new DemoParserService(mapCatalog);

        // FIX: ParseAsync принимает (path, sampleHz, ct) — второй аргумент float, не CancellationToken
        var timeline = await parser.ParseAsync(filePath, 16f, ct);

        return ConvertTimeline(timeline, filePath);
    }

    private static DemoData ConvertTimeline(
        Cs2DemoViewer.Core.Models.DemoTimeline timeline,
        string filePath)
    {
        var rounds = new List<RoundData>();

        foreach (var round in timeline.Rounds)
        {
            var frames = new List<FrameData>();

            foreach (var frame in round.Frames)
            {
                var players = frame.Actors.Select(p => new PlayerData
                {
                    Slot          = p.Slot,
                    // FIX: Actor не содержит Name — генерируем из Slot
                    Name          = $"Player {p.Slot}",
                    IsCt          = p.IsCt,
                    Health        = p.Health,
                    Armor         = p.Armor,
                    IsAlive       = p.Alive,
                    X             = p.X,
                    Y             = p.Y,
                    Yaw           = p.Yaw,
                    Weapon        = p.ActiveWeapon ?? "",
                    Money         = p.Money,
                    HasHelmet     = p.Helmet,
                    HasDefuser    = p.Defuser,
                    // FIX: Actor не содержит Blind — используем FlashDuration > 0
                    IsBlind       = p.FlashDuration > 0,
                    FlashDuration = p.FlashDuration,
                    // FIX: Actor не содержит SteamId — оставляем 0
                    SteamId       = 0,
                }).ToList();

                // FIX: Frame не содержит Areas — гранаты берём из Projectiles
                var grenades = frame.Projectiles.Select(a => new GrenadeData
                {
                    Kind = a.Kind.ToString(),
                    X    = a.X,
                    Y    = a.Y,
                }).ToList();

                frames.Add(new FrameData
                {
                    Time     = frame.Time,
                    Players  = players,
                    Grenades = grenades,
                    // FIX: Frame не содержит Bomb — бомба хранится в RoundTimeline.Bomb
                    BombX    = null,
                    BombY    = null,
                });
            }

            var kills = round.Kills.Select(k => new KillData
            {
                AttackerSlot = k.AttackerSlot,
                VictimSlot   = k.VictimSlot,
                Weapon       = k.Weapon ?? "",
                IsHeadshot   = k.Headshot,
                AttackerCt   = k.AttackerCt,
                VictimCt     = k.VictimCt,
                Time         = k.Time,
            }).ToList();

            rounds.Add(new RoundData
            {
                Number    = round.Number,
                // FIX: RoundTimeline не содержит Start/End — используем Duration
                StartTime = 0f,
                EndTime   = round.Duration,
                // FIX: RoundTimeline не содержит Winner — вычисляем из CtWon
                Winner    = (round.CtWon ?? false) ? 2 : 3,
                // FIX: CtWon — это bool?, нужно явное приведение
                CtWon     = round.CtWon ?? false,
                ScoreCt   = round.ScoreCt,
                ScoreT    = round.ScoreT,
                // FIX: RoundTimeline не содержит Outcome — берём из BombInfo если есть
                Outcome   = round.Bomb?.Outcome.ToString() ?? "",
                Frames    = frames,
                Kills     = kills,
            });
        }

        return new DemoData
        {
            FileName  = Path.GetFileName(filePath),
            FilePath  = filePath,
            MapName   = timeline.Map ?? "unknown",
            // FIX: DemoTimeline не содержит CtClan/TClan
            CtClan    = "CT",
            TClan     = "T",
            Rounds    = rounds,
            Players   = timeline.Players.Select(p => new PlayerSummary
            {
                Slot    = p.Slot,
                Name    = p.Name ?? $"Player {p.Slot}",
                // FIX: PlayerInfo не содержит IsCt
                IsCt    = false,
                SteamId = p.SteamId,
            }).ToList(),
        };
    }

    public IReadOnlyList<RecentDemo> GetRecentDemos() => _recent.AsReadOnly();

    public void AddToRecent(string filePath, string mapName, int roundCount)
    {
        _recent.RemoveAll(r => r.FilePath == filePath);
        _recent.Insert(0, new RecentDemo
        {
            FilePath   = filePath,
            FileName   = Path.GetFileName(filePath),
            MapName    = mapName,
            RoundCount = roundCount,
            OpenedAt   = DateTime.Now,
        });

        while (_recent.Count > 10) _recent.RemoveAt(_recent.Count - 1);
        SaveRecent();
    }

    private void LoadRecent()
    {
        try
        {
            var json = Preferences.Get(RecentKey, "");
            if (!string.IsNullOrEmpty(json))
            {
                var list = System.Text.Json.JsonSerializer.Deserialize<List<RecentDemo>>(json);
                if (list != null) _recent.AddRange(list);
            }
        }
        catch { /* первый запуск */ }
    }

    private void SaveRecent()
    {
        var json = System.Text.Json.JsonSerializer.Serialize(_recent);
        Preferences.Set(RecentKey, json);
    }
}
