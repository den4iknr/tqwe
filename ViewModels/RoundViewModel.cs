using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cs2DemoViewer.Maui.Models;
using Cs2DemoViewer.Maui.Services;

namespace Cs2DemoViewer.Maui.ViewModels;

[QueryProperty(nameof(Round), "Round")]
[QueryProperty(nameof(Demo),  "Demo")]
public partial class RoundViewModel : ObservableObject
{
    private readonly IMapService _mapService;

    [ObservableProperty] private RoundData?  _round;
    [ObservableProperty] private DemoData?   _demo;
    [ObservableProperty] private FrameData?  _currentFrame;
    [ObservableProperty] private int         _frameIndex;
    [ObservableProperty] private bool        _isPlaying;
    [ObservableProperty] private string?     _mapImageName;
    [ObservableProperty] private double      _mapSize = 1024;

    private CancellationTokenSource? _playCts;

    public System.Collections.ObjectModel.ObservableCollection<PlayerData>  Players  { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<GrenadeData> Grenades { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<KillData>    Kills    { get; } = new();

    public int FrameCount => Round?.Frames.Count ?? 0;

    public RoundViewModel(IMapService mapService)
    {
        _mapService = mapService;
    }

    partial void OnRoundChanged(RoundData? value)
    {
        if (value is null) return;
        FrameIndex = 0;
        Kills.Clear();
        foreach (var k in value.Kills) Kills.Add(k);
        ApplyFrame(0);
        OnPropertyChanged(nameof(FrameCount));
    }

    partial void OnDemoChanged(DemoData? value)
    {
        if (value is null) return;
        MapImageName = _mapService.GetMapImageName(value.MapName);
    }

    partial void OnFrameIndexChanged(int value)
    {
        ApplyFrame(value);
    }

    private void ApplyFrame(int index)
    {
        if (Round is null || Round.Frames.Count == 0) return;
        index = Math.Clamp(index, 0, Round.Frames.Count - 1);

        CurrentFrame = Round.Frames[index];

        Players.Clear();
        foreach (var p in CurrentFrame.Players) Players.Add(p);

        Grenades.Clear();
        foreach (var g in CurrentFrame.Grenades) Grenades.Add(g);
    }

    // ── Воспроизведение ──────────────────────────────────────────

    [RelayCommand]
    private async Task TogglePlayAsync()
    {
        if (IsPlaying)
        {
            _playCts?.Cancel();
            IsPlaying = false;
            return;
        }

        IsPlaying = true;
        _playCts  = new CancellationTokenSource();
        var ct    = _playCts.Token;

        try
        {
            while (!ct.IsCancellationRequested && FrameIndex < FrameCount - 1)
            {
                FrameIndex++;
                // ~16 FPS воспроизведение
                await Task.Delay(62, ct);
            }
        }
        catch (TaskCanceledException) { }
        finally
        {
            IsPlaying = false;
        }
    }

    [RelayCommand]
    private void StepBack()
    {
        _playCts?.Cancel();
        if (FrameIndex > 0) FrameIndex--;
    }

    [RelayCommand]
    private void StepForward()
    {
        _playCts?.Cancel();
        if (FrameIndex < FrameCount - 1) FrameIndex++;
    }

    [RelayCommand]
    private void GoToStart()
    {
        _playCts?.Cancel();
        FrameIndex = 0;
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        _playCts?.Cancel();
        IsPlaying = false;
        await Shell.Current.GoToAsync("..");
    }
}
