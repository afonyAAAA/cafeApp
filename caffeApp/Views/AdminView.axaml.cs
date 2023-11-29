using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using caffeApp.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace caffeApp.Views
{
    public partial class AdminView : ReactiveUserControl<AdminViewModel>
    {
        public AdminView()
        {
            this.WhenActivated(disposables => {

            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
