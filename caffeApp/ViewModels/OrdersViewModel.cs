using caffeApp.ViewModels.Waiter;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.ViewModels
{
    public class OrdersViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get; set; }
        public override IScreen HostScreen { get; set; }
        public override ViewModelActivator Activator { get; set; }

        public ReactiveCommand<Unit, Unit> OpenAddOrderView { get; }

        public OrdersViewModel(IScreen screen)
        {
            Activator = new();

            HostScreen = screen;

            OpenAddOrderView = ReactiveCommand.Create(() => {
                HostScreen.Router.Navigate.Execute(new AddOrderViewModel(HostScreen));
            });
        }

    }
}
