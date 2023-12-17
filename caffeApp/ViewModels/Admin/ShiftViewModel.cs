using Avalonia.Controls.Primitives;
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
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.ViewModels.Admin
{
    public class ShiftViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override IScreen HostScreen { get; set; }

        public override ViewModelActivator Activator { get; set; }

        private ObservableCollection<Workshiftview> _userWorkshifts;

        public ReactiveCommand<Unit, Unit> OpenAddWorkShift { get; }
        public ReactiveCommand<Unit, Unit> UnselectWorkshift { get; }

        private Workshiftview _selectedWorkShift;

        private List<Workshiftview> _usersInWorkShift;

        private bool _isClickOnWorkShift = false;
        
        public ObservableCollection<Workshiftview> UserWorkShifts {
            get => _userWorkshifts;
            set => this.RaiseAndSetIfChanged(ref _userWorkshifts, value);
        }

        public List<Workshiftview> UsersInWorkShift { 
            get => _usersInWorkShift; 
            set => this.RaiseAndSetIfChanged(ref _usersInWorkShift, value); 
        }
        public bool IsClickOnWorkShift {
            get => _isClickOnWorkShift; 
            set => this.RaiseAndSetIfChanged(ref _isClickOnWorkShift, value);
        }
        public Workshiftview SelectedWorkShift {
            get => _selectedWorkShift;
            set => this.RaiseAndSetIfChanged(ref _selectedWorkShift, value);
        }

        public ShiftViewModel(IScreen screen)
        {
            Activator = new();

            HostScreen = screen;

            var shifts = DatabaseHelper.refreshEntity<Workshiftview>().ToList();

            var groupedShifts = new List<Workshiftview>();

            Workshiftview prevShift = null;

            foreach (var shift in shifts)
            {
                if (prevShift == null || shift.WorkshiftId != prevShift.WorkshiftId)
                {
                    groupedShifts.Add(shift);
                }
                prevShift = shift;
            }

            UserWorkShifts = new ObservableCollection<Workshiftview>(groupedShifts);

            OpenAddWorkShift = ReactiveCommand.Create(() => {
                HostScreen.Router.Navigate.Execute(new AddWorkShiftViewModel(HostScreen));
            });

            UnselectWorkshift = ReactiveCommand.Create(() => {
                IsClickOnWorkShift = false;
            });

            this.WhenAnyValue(x => x.IsClickOnWorkShift).Subscribe(_ => {
                if(SelectedWorkShift != null)
                {
                    UsersInWorkShift = DatabaseHelper.refreshEntity<Workshiftview>().Where(x => x.WorkshiftId == SelectedWorkShift.WorkshiftId).ToList();
                }  
            });

            this.WhenAnyValue(x => x.SelectedWorkShift).Subscribe(_ =>
            {
                if(SelectedWorkShift != null)
                {
                    IsClickOnWorkShift = true;
                }
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
