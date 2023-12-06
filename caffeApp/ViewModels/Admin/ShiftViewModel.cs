using caffeApp.Desktop;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.ViewModels.Admin
{
    public class ShiftViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IScreen HostScreen { get; set; }
        public override ViewModelActivator Activator { get; set; }
        public ObservableCollection<Workshift> Shifts { 
            get => _shifts; 
            set => this.RaiseAndSetIfChanged(ref _shifts, value);
        }

        private ObservableCollection<Workshift> _shifts;

        public ShiftViewModel(IScreen screen)
        {
            Activator = new();

            HostScreen = screen;




        }

    }
}
