using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using caffeApp.ViewModels;
using ReactiveUI;

namespace caffeApp.Views
{
    public partial class AuthorizationView : ReactiveUserControl<AuthorizationViewModel>
    {
        public AuthorizationView()
        {
            this.WhenActivated(disposables =>
            {

            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
