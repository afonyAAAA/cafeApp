using caffeApp.Desktop;
using caffeApp.Sources;
using caffeApp.utils;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.ViewModels.Admin
{
    public class ShiftViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override IScreen HostScreen { get; set; }

        public override ViewModelActivator Activator { get; set; }

        private ObservableCollection<Workshift> _shifts;

        public ReactiveCommand<Unit, Unit> OpenAddWorkShift { get; }

        public ObservableCollection<Workshift> Shifts { 
            get => _shifts; 
            set => this.RaiseAndSetIfChanged(ref _shifts, value);
        }

        public ShiftViewModel(IScreen screen)
        {
            Activator = new();

            HostScreen = screen;

            Shifts = DatabaseHelper.refreshEntity<Workshift>();

            OpenAddWorkShift = ReactiveCommand.Create(() => {
                HostScreen.Router.Navigate.Execute(new AddWorkShiftViewModel(HostScreen));
            });

            this.WhenActivated((disposables) =>
            {
                /* handle activation */
                Disposable
                    .Create(() =>
                    {
                    })
                    .DisposeWith(disposables);
            });
        }
    }
}
