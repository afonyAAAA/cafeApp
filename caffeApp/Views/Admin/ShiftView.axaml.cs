using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using caffeApp.ViewModels.Admin;

namespace caffeApp.Views.Admin
{
    public partial class ShiftView : ReactiveUserControl<ShiftViewModel>
    {
        public ShiftView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
