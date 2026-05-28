using Cs2DemoViewer.Maui.Models;
using Cs2DemoViewer.Maui.ViewModels;

namespace Cs2DemoViewer.Maui.Views;

public partial class RoundPage : ContentPage
{
    private RoundViewModel Vm => (RoundViewModel)BindingContext;

    public RoundPage(RoundViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // Кнопка "📋" — показывает лист убийств раунда
    private async void ShowKillsClicked(object sender, EventArgs e)
    {
        if (Vm.Kills.Count == 0)
        {
            await DisplayAlert("Убийства", "В этом раунде убийств не зафиксировано", "OK");
            return;
        }

        var lines = Vm.Kills.Select((k, i) =>
        {
            var hs   = k.IsHeadshot ? " 🎯HS" : "";
            var side = k.AttackerCt ? "[CT]" : "[T]";
            return $"{i + 1}. {side} слот {k.AttackerSlot} → слот {k.VictimSlot}  {k.Weapon}{hs}  @{k.Time:F1}s";
        });

        var text = string.Join("\n", lines);
        await DisplayAlert($"Убийства — Раунд {Vm.Round?.Number}", text, "Закрыть");
    }
}
