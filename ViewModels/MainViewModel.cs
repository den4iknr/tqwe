using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cs2DemoViewer.Maui.Models;
using Cs2DemoViewer.Maui.Services;

namespace Cs2DemoViewer.Maui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IDemoService _demoService;

    [ObservableProperty] private bool   _isLoading;
    [ObservableProperty] private string _loadingText = "";
    [ObservableProperty] private string _errorText   = "";

    public System.Collections.ObjectModel.ObservableCollection<RecentDemo> RecentDemos { get; } = new();

    public MainViewModel(IDemoService demoService)
    {
        _demoService = demoService;
        LoadRecent();
    }

    private void LoadRecent()
    {
        RecentDemos.Clear();
        foreach (var demo in _demoService.GetRecentDemos())
            RecentDemos.Add(demo);
    }

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        ErrorText = "";
        try
        {
            // Открываем системный выбор файлов — .dem файлы
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Выбери .dem файл",
                FileTypes   = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/octet-stream", "*/*" } },
                    { DevicePlatform.iOS,     new[] { "public.data" } },
                })
            });

            if (result is null) return;

            await LoadDemoAsync(result.FullPath);
        }
        catch (Exception ex)
        {
            ErrorText = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task OpenRecentAsync(RecentDemo demo)
    {
        if (!File.Exists(demo.FilePath))
        {
            ErrorText = "Файл не найден. Возможно он был перемещён или удалён.";
            return;
        }
        await LoadDemoAsync(demo.FilePath);
    }

    private async Task LoadDemoAsync(string filePath)
    {
        IsLoading   = true;
        LoadingText = "Читаем демку...";
        ErrorText   = "";

        try
        {
            LoadingText = "Парсим события...";
            var demo = await _demoService.ParseDemoAsync(filePath);

            _demoService.AddToRecent(filePath, demo.MapName, demo.RoundCount);
            LoadRecent();

            // Передаём данные на следующий экран через Shell navigation
            await Shell.Current.GoToAsync("DemoPage", new Dictionary<string, object>
            {
                ["Demo"] = demo
            });
        }
        catch (Exception ex)
        {
            ErrorText = $"Не удалось открыть файл: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
