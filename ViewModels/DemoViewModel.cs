using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cs2DemoViewer.Maui.Models;

namespace Cs2DemoViewer.Maui.ViewModels;

[QueryProperty(nameof(Demo), "Demo")]
public partial class DemoViewModel : ObservableObject
{
    [ObservableProperty] private DemoData? _demo;
    [ObservableProperty] private RoundData? _selectedRound;

    public System.Collections.ObjectModel.ObservableCollection<RoundData> Rounds { get; } = new();

    partial void OnDemoChanged(DemoData? value)
    {
        if (value is null) return;
        Rounds.Clear();
        foreach (var r in value.Rounds)
            Rounds.Add(r);
    }

    [RelayCommand]
    private async Task OpenRoundAsync(RoundData round)
    {
        if (Demo is null) return;
        SelectedRound = round;

        await Shell.Current.GoToAsync("RoundPage", new Dictionary<string, object>
        {
            ["Round"]   = round,
            ["Demo"]    = Demo,
        });
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
