using System.Collections.ObjectModel;
using Cs2DemoViewer.Maui.Models;

namespace Cs2DemoViewer.Maui.Views;

/// <summary>
/// Рисует позиции игроков, гранат и бомбы поверх изображения карты.
/// Использует MAUI GraphicsView (нативный Canvas).
/// </summary>
public class MapCanvas : GraphicsView, IDrawable
{
    // ── Bindable Properties ──────────────────────────────────────

    public static readonly BindableProperty PlayersProperty =
        BindableProperty.Create(nameof(Players), typeof(ObservableCollection<PlayerData>),
            typeof(MapCanvas), null, propertyChanged: (b, _, _) => ((MapCanvas)b).Invalidate());

    public static readonly BindableProperty GrenadesProperty =
        BindableProperty.Create(nameof(Grenades), typeof(ObservableCollection<GrenadeData>),
            typeof(MapCanvas), null, propertyChanged: (b, _, _) => ((MapCanvas)b).Invalidate());

    public static readonly BindableProperty BombXProperty =
        BindableProperty.Create(nameof(BombX), typeof(float?), typeof(MapCanvas), null,
            propertyChanged: (b, _, _) => ((MapCanvas)b).Invalidate());

    public static readonly BindableProperty BombYProperty =
        BindableProperty.Create(nameof(BombY), typeof(float?), typeof(MapCanvas), null,
            propertyChanged: (b, _, _) => ((MapCanvas)b).Invalidate());

    public static readonly BindableProperty MapSizeProperty =
        BindableProperty.Create(nameof(MapSize), typeof(double), typeof(MapCanvas), 1024.0,
            propertyChanged: (b, _, _) => ((MapCanvas)b).Invalidate());

    public ObservableCollection<PlayerData>?  Players  { get => (ObservableCollection<PlayerData>?)GetValue(PlayersProperty);  set => SetValue(PlayersProperty, value); }
    public ObservableCollection<GrenadeData>? Grenades { get => (ObservableCollection<GrenadeData>?)GetValue(GrenadesProperty); set => SetValue(GrenadesProperty, value); }
    public float? BombX { get => (float?)GetValue(BombXProperty); set => SetValue(BombXProperty, value); }
    public float? BombY { get => (float?)GetValue(BombYProperty); set => SetValue(BombYProperty, value); }
    public double MapSize { get => (double)GetValue(MapSizeProperty); set => SetValue(MapSizeProperty, value); }

    public MapCanvas()
    {
        Drawable = this;
        BackgroundColor = Colors.Transparent;
    }

    // ── Отрисовка ────────────────────────────────────────────────

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var w = dirtyRect.Width;
        var h = dirtyRect.Height;
        var scale = (float)Math.Min(w, h) / (float)MapSize;

        DrawGrenades(canvas, scale, w, h);
        DrawBomb(canvas, scale, w, h);
        DrawPlayers(canvas, scale, w, h);
    }

    private void DrawPlayers(ICanvas canvas, float scale, float w, float h)
    {
        if (Players is null) return;

        foreach (var p in Players)
        {
            // Конвертируем мировые координаты в экранные
            // Карта CS2: X идёт вправо, Y идёт вниз (в файле уже сконвертировано)
            var sx = p.X * scale;
            var sy = p.Y * scale;

            if (sx < 0 || sx > w || sy < 0 || sy > h) continue;

            float r = p.IsAlive ? 10f : 7f;

            // Тень
            canvas.FillColor  = Color.FromArgb("#44000000");
            canvas.FillCircle(sx + 1, sy + 1, r);

            // Заливка
            canvas.FillColor = p.IsAlive
                ? (p.IsCt ? Color.FromArgb("#4a9eff") : Color.FromArgb("#ffaa00"))
                : Color.FromArgb("#555555");
            canvas.FillCircle(sx, sy, r);

            // Направление взгляда (Yaw)
            if (p.IsAlive)
            {
                var yawRad = p.Yaw * MathF.PI / 180f;
                var ex = sx + MathF.Cos(yawRad) * r * 1.5f;
                var ey = sy + MathF.Sin(yawRad) * r * 1.5f;
                canvas.StrokeColor = Colors.White;
                canvas.StrokeSize  = 2f;
                canvas.DrawLine(sx, sy, ex, ey);
            }

            // Обводка
            canvas.StrokeColor = p.IsAlive ? Colors.White : Colors.DimGray;
            canvas.StrokeSize  = 1.5f;
            canvas.DrawCircle(sx, sy, r);

            // Ник (сокращённый)
            if (p.IsAlive)
            {
                canvas.FontSize  = 9f;
                canvas.FontColor = Colors.White;
                var shortName = p.Name.Length > 8 ? p.Name[..8] : p.Name;
                canvas.DrawString(shortName, sx - 24, sy + r + 2, 48, 14,
                    HorizontalAlignment.Center, VerticalAlignment.Top);
            }

            // Полоска HP
            if (p.IsAlive && p.Health < 100)
            {
                float barW  = 20f;
                float barH  = 3f;
                float barX  = sx - barW / 2;
                float barY  = sy - r - 6;
                float filled = barW * (p.Health / 100f);

                canvas.FillColor = Color.FromArgb("#333333");
                canvas.FillRectangle(barX, barY, barW, barH);
                canvas.FillColor = p.Health > 50 ? Color.FromArgb("#4caf50") : Color.FromArgb("#f44336");
                canvas.FillRectangle(barX, barY, filled, barH);
            }

            // Флаг ослепления
            if (p.IsBlind && p.FlashDuration > 0)
            {
                canvas.FontSize  = 10f;
                canvas.FontColor = Colors.Yellow;
                canvas.DrawString("💥", sx + r, sy - r - 2, 14, 14,
                    HorizontalAlignment.Left, VerticalAlignment.Top);
            }
        }
    }

    private void DrawGrenades(ICanvas canvas, float scale, float w, float h)
    {
        if (Grenades is null) return;

        foreach (var g in Grenades)
        {
            var sx = g.X * scale;
            var sy = g.Y * scale;
            if (sx < 0 || sx > w || sy < 0 || sy > h) continue;

            canvas.FillColor = g.Kind switch
            {
                "Flash"   => Color.FromArgb("#fffde7"),
                "HE"      => Color.FromArgb("#e53935"),
                "Smoke"   => Color.FromArgb("#78909c"),
                "Molotov" or "Inferno" => Color.FromArgb("#ff6d00"),
                "Decoy"   => Color.FromArgb("#ab47bc"),
                _         => Color.FromArgb("#ffffff"),
            };
            canvas.FillCircle(sx, sy, 5f);
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize  = 1f;
            canvas.DrawCircle(sx, sy, 5f);
        }
    }

    private void DrawBomb(ICanvas canvas, float scale, float w, float h)
    {
        if (BombX is null || BombY is null) return;
        var sx = BombX.Value * scale;
        var sy = BombY.Value * scale;
        if (sx < 0 || sx > w || sy < 0 || sy > h) return;

        // Мигающий красный квадрат для бомбы
        canvas.FillColor   = Color.FromArgb("#ff1744");
        canvas.StrokeColor = Colors.White;
        canvas.StrokeSize  = 2f;
        canvas.FillRectangle(sx - 6, sy - 6, 12, 12);
        canvas.DrawRectangle(sx - 6, sy - 6, 12, 12);
        canvas.FontSize  = 9f;
        canvas.FontColor = Colors.White;
        canvas.DrawString("B", sx - 4, sy - 5, 8, 10,
            HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
