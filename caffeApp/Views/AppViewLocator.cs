using caffeApp.ViewModels;
using caffeApp.ViewModels.Admin;
using caffeApp.Views.Admin;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.Views
{
    public class AppViewLocator : IViewLocator
    {
        public IViewFor ResolveView<T>(T viewModel, string contract = null) => viewModel switch
        {
            UsersViewModel context => new UsersView { DataContext = context },
            AuthorizationViewModel context => new AuthorizationView { DataContext = context},
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
