using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using caffeApp.ViewModels.Admin;
using ReactiveUI;
using System.Reactive.Disposables;

namespace caffeApp.Views.Admin
{
    public partial class UsersView : ReactiveUserControl<UsersViewModel>
    {
        public UsersView()
        {
            this.WhenActivated(disposables =>
            {

            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
