using caffeApp.ViewModels;
using caffeApp.ViewModels.Admin;
using caffeApp.Views.Admin;
using ReactiveUI;
using System;

namespace caffeApp.Views
{
    public class AppViewLocator : IViewLocator
    {
        public IViewFor ResolveView<T>(T viewModel, string contract = null) => viewModel switch
        {
            UsersViewModel context => new UsersView { DataContext = context },
            AuthorizationViewModel context => new AuthorizationView { DataContext = context},
            RegistrationViewModel context => new RegistrationView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
