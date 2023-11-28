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
        public Grid GridUser => this.FindControl<Grid>("grid_selected_user");

        public AdminView()
        {
            this.WhenActivated( disposables => {
                this.Bind(ViewModel, x => x.GridSelectedUser, x => x.GridUser)
                     .DisposeWith(disposables);
            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
