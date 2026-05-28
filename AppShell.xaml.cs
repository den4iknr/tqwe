using Cs2DemoViewer.Maui.Views;

namespace Cs2DemoViewer.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Регистрируем маршруты для навигации
        Routing.RegisterRoute("DemoPage", typeof(DemoPage));
        Routing.RegisterRoute("RoundPage", typeof(RoundPage));
    }
}
