using Cs2DemoViewer.Maui.ViewModels;

namespace Cs2DemoViewer.Maui.Views;

public partial class DemoPage : ContentPage
{
    public DemoPage(DemoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
