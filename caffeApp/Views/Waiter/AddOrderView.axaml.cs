using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using caffeApp.ViewModels.Waiter;

namespace caffeApp.Views.Waiter
{
    public partial class AddOrderView : ReactiveUserControl<AddOrderViewModel>
    {
        public AddOrderView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
