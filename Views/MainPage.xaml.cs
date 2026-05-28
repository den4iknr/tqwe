using Cs2DemoViewer.Maui.ViewModels;

namespace Cs2DemoViewer.Maui.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
