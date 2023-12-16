using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using caffeApp.ViewModels;

namespace caffeApp.Views
{
    public partial class ReportView : ReactiveUserControl<ReportViewModel>
    {
        public ReportView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
