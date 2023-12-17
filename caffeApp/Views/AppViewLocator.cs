using caffeApp.ViewModels;
using caffeApp.ViewModels.Admin;
using caffeApp.ViewModels.Waiter;
using caffeApp.Views.Admin;
using caffeApp.Views.Waiter;
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
            ShiftViewModel context => new ShiftView { DataContext = context },
            AddWorkShiftViewModel context => new AddWorkShiftView { DataContext = context},
            AddOrderViewModel context => new AddOrderView {  DataContext = context },
            OrdersViewModel context => new OrdersView {DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
