using Avalonia.Controls;
using caffeApp.ViewModels;

namespace caffeApp.Views
{
    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            DataContext = new AdminViewModel(this);
            InitializeComponent();
        }
    }
}
