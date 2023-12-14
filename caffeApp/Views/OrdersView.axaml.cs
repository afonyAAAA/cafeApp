using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using caffeApp.ViewModels;

namespace caffeApp.Views
{
    public partial class OrdersView : ReactiveUserControl<OrdersViewModel>
    {
        public OrdersView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
