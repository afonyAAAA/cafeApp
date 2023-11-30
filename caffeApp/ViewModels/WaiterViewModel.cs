using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.ViewModels
{
    public class WaiterViewModel : ViewModelBase
    {
        public string Hello => "Waiter";

        public override string? UrlPathSegment { 
            get => UrlPathSegment;
            set => UrlPathSegment = value; 
        }
        public override IScreen HostScreen {
            get => HostScreen;
            set => HostScreen = value; 
        }
        public override ViewModelActivator Activator {
            get => Activator;
            set => Activator = value;
        }

        public WaiterViewModel(IScreen screen) {
            HostScreen = screen;
        }  
    }
}
