using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using caffeApp.ViewModels.Admin;
using ReactiveUI;

namespace caffeApp.Views.Admin
{
    public partial class RegistrationView : ReactiveUserControl<RegistrationViewModel>
    {
        public RegistrationView()
        {
            this.WhenActivated(disposables =>
            {

            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
