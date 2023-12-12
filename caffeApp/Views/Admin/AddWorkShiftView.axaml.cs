using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using caffeApp.ViewModels.Admin;
using ReactiveUI;

namespace caffeApp.Views.Admin
{
    public partial class AddWorkShiftView : ReactiveUserControl<AddWorkShiftViewModel>
    {
        public AddWorkShiftView()
        {
            this.WhenActivated(disposables =>
            {

            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
