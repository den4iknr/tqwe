namespace Cs2DemoViewer.Maui.Models;

// ─── Демка целиком ────────────────────────────────────────────────

public class DemoData
{
    public string FileName  { get; set; } = "";
    public string FilePath  { get; set; } = "";
    public string MapName   { get; set; } = "";
    public string CtClan    { get; set; } = "CT";
    public string TClan     { get; set; } = "T";
    public List<RoundData>       Rounds  { get; set; } = new();
    public List<PlayerSummary>   Players { get; set; } = new();

    public int CtScore => Rounds.LastOrDefault()?.ScoreCt ?? 0;
    public int TScore  => Rounds.LastOrDefault()?.ScoreT  ?? 0;
    public int RoundCount => Rounds.Count;
}

// ─── Раунд ───────────────────────────────────────────────────────

public class RoundData
{
    public int     Number    { get; set; }
    public float   StartTime { get; set; }
    public float   EndTime   { get; set; }
    public int     Winner    { get; set; }  // 2=CT 3=T
    public bool    CtWon     { get; set; }
    public int     ScoreCt   { get; set; }
    public int     ScoreT    { get; set; }
    public string  Outcome   { get; set; } = "";
    public List<FrameData> Frames { get; set; } = new();
    public List<KillData>  Kills  { get; set; } = new();

    public string ScoreDisplay => $"{ScoreCt} : {ScoreT}";
    public string WinnerLabel  => CtWon ? "CT Win" : "T Win";
    public float  Duration     => EndTime - StartTime;
}

// ─── Кадр (снимок позиций в момент времени) ──────────────────────

public class FrameData
{
    public float  Time     { get; set; }
    public List<PlayerData>  Players  { get; set; } = new();
    public List<GrenadeData> Grenades { get; set; } = new();
    public float? BombX    { get; set; }
    public float? BombY    { get; set; }
}

// ─── Игрок в кадре ───────────────────────────────────────────────

public class PlayerData
{
    public int    Slot          { get; set; }
    public string Name          { get; set; } = "";
    public bool   IsCt          { get; set; }
    public int    Health        { get; set; }
    public int    Armor         { get; set; }
    public bool   IsAlive       { get; set; }
    public float  X             { get; set; }
    public float  Y             { get; set; }
    public float  Yaw           { get; set; }
    public string Weapon        { get; set; } = "";
    public int    Money         { get; set; }
    public bool   HasHelmet     { get; set; }
    public bool   HasDefuser    { get; set; }
    public bool   IsBlind       { get; set; }
    public float  FlashDuration { get; set; }
    public ulong  SteamId       { get; set; }

    public string TeamColor     => IsCt ? "#4a9eff" : "#ffaa00";
    public string HealthBar     => Health <= 0 ? "💀" : $"❤ {Health}";
}

// ─── Граната в кадре ─────────────────────────────────────────────

public class GrenadeData
{
    public string Kind { get; set; } = "";  // Flash, HE, Smoke, Molotov, Decoy
    public float  X    { get; set; }
    public float  Y    { get; set; }

    public string Emoji => Kind switch
    {
        "Flash"   => "💥",
        "HE"      => "💣",
        "Smoke"   => "💨",
        "Molotov" => "🔥",
        "Inferno" => "🔥",
        "Decoy"   => "📢",
        _         => "●",
    };
}

// ─── Убийство ────────────────────────────────────────────────────

public class KillData
{
    public int    AttackerSlot { get; set; }
    public int    VictimSlot   { get; set; }
    public string Weapon       { get; set; } = "";
    public bool   IsHeadshot   { get; set; }
    public bool   AttackerCt   { get; set; }
    public bool   VictimCt     { get; set; }
    public float  Time         { get; set; }

    public string Display => $"{(IsHeadshot ? "🎯" : "💀")} {Weapon}";
}

// ─── Итоговая статистика игрока ──────────────────────────────────

public class PlayerSummary
{
    public int    Slot    { get; set; }
    public string Name    { get; set; } = "";
    public bool   IsCt    { get; set; }
    public ulong  SteamId { get; set; }
}

// ─── Недавно открытые ────────────────────────────────────────────

public class RecentDemo
{
    public string   FilePath   { get; set; } = "";
    public string   FileName   { get; set; } = "";
    public string   MapName    { get; set; } = "";
    public int      RoundCount { get; set; }
    public DateTime OpenedAt   { get; set; }

    public string TimeAgo
    {
        get
        {
            var diff = DateTime.Now - OpenedAt;
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} мин назад";
            if (diff.TotalHours   < 24) return $"{(int)diff.TotalHours} ч назад";
            return $"{(int)diff.TotalDays} дн назад";
        }
    }
}
