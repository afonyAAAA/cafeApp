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
        public Grid GridUser => this.Find<Grid>("grid_selected_user");

        public AdminView()
        {
            this.WhenActivated( disposables => {
                this.OneWayBind(ViewModel, x => x.GridSelectedUser, x => x.GridUser)
                     .DisposeWith(disposables);
            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
