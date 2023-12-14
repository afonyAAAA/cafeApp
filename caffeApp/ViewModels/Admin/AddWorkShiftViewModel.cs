using caffeApp.Desktop;
using caffeApp.utils;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Internal;
using Microsoft.VisualBasic;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using NodaTime;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using caffeApp.Sources;
using DynamicData;
using System.Reactive.Linq;

namespace caffeApp.ViewModels.Admin
{
    public class AddWorkShiftViewModel : ViewModelBase
    {
        private readonly LocalDateTime currentDateTime = NodaTime.SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime;
     
        public override string? UrlPathSegment { get; set; }
        public override IScreen HostScreen { get; set; }
        public override ViewModelActivator Activator { get; set; }

        private DateTimeOffset _selectedDate;
        private TimeSpan _selectedTimeStart;
        private TimeSpan _selectedTimeEnd;
        private User _selectedFromAllUsers;
        private User _selectedFromSelectedUsers;
        private IObservable<bool> _deleteIsClickable;
        private IObservable<bool> _addIsClickable;

        private ObservableCollection<User> _users;
        private ObservableCollection<Workshift> _workShifts;
        private ObservableCollection<User> _selectedUsers;

        public ReactiveCommand<Unit, Unit> AddUserShift { get; }
        public ReactiveCommand<Unit, Unit> DeleteUserShift { get; }
        public ReactiveCommand<Unit, Unit> Submit { get; }

        public ObservableCollection<User> Users {
            get => _users;
            set => this.RaiseAndSetIfChanged(ref _users, value);
        }

        public ObservableCollection<Workshift> WorkShifts { 
            get => _workShifts; 
            set => this.RaiseAndSetIfChanged(ref _workShifts, value);
        }

        public ObservableCollection<User> SelectedUsers {
            get => _selectedUsers;
            set => this.RaiseAndSetIfChanged(ref _selectedUsers, value); 
        }

        public DateTimeOffset SelectedDate { 
            get => _selectedDate; 
            set => this.RaiseAndSetIfChanged(ref _selectedDate, value);
        }

        public TimeSpan SelectedTimeStart { 
            get => _selectedTimeStart;
            set => this.RaiseAndSetIfChanged(ref _selectedTimeStart, value);
        }

        public TimeSpan SelectedTimeEnd {
            get => _selectedTimeEnd;
            set => this.RaiseAndSetIfChanged(ref _selectedTimeEnd, value);
        }

        public User SelectedFromAllUsers { 
            get => _selectedFromAllUsers;
            set => this.RaiseAndSetIfChanged(ref _selectedFromAllUsers, value);
        }    

        public User SelectedFromSelectedUsers
        { 
            get => _selectedFromSelectedUsers;
            set => this.RaiseAndSetIfChanged(ref _selectedFromSelectedUsers, value);
        }

        public IObservable<bool> DeleteIsClickable {
            get => _deleteIsClickable;
            set => this.RaiseAndSetIfChanged(ref _deleteIsClickable, value);
        }

        public IObservable<bool> AddIsClickable { 
            get => _addIsClickable; 
            set => this.RaiseAndSetIfChanged(ref _addIsClickable, value);
        }
        public AddWorkShiftViewModel(IScreen screen) {

            Activator = new ViewModelActivator();
            HostScreen = screen;
            Users = DatabaseHelper.refreshEntity<User>(x => x.Role);
            WorkShifts = DatabaseHelper.refreshEntity<Workshift>();

            SelectedDate = new DateTimeOffset(new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day));
         
            AddIsClickable = this.WhenAnyValue(x => x.Users.Count).Select(x => x != 0);

            DeleteIsClickable = this.WhenAnyValue(x => x.SelectedUsers.Count).Select(x => x != 0);

            Submit = ReactiveCommand.Create(() =>
            {
                if (CheckDate().Result)
                {
                    CreateWorkShift();
                    HostScreen.Router.Navigate.Execute(new ShiftViewModel(HostScreen));
                }
            });

            AddUserShift = ReactiveCommand.Create(() => {
                AddUserInShift();
            }, AddIsClickable);

            DeleteUserShift = ReactiveCommand.Create(() => {
                DeleteUserInShift();
            }, DeleteIsClickable);

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

        private async Task CreateWorkShift()
        {
            var context = DbContextProvider.GetContext();
            Workshift workshift = new();
            workshift.Date = new DateOnly(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day);
            workshift.Timestart = new TimeOnly(SelectedTimeStart.Hours, SelectedTimeStart.Minutes);
            workshift.Timeend = new TimeOnly(SelectedTimeEnd.Hours, SelectedTimeEnd.Minutes);
            var createdWorkShift = context.Workshifts.Add(workshift);
            context.SaveChanges();

            if(SelectedUsers.Count != 0)
            {
                List<UserWorkShift> workshifts = new List<UserWorkShift>();
                foreach(var user in  SelectedUsers)
                {
                    UserWorkShift userworkshift = new UserWorkShift();
                    userworkshift.WorkshiftId = createdWorkShift.Entity.WorkshiftId;
                    userworkshift.UserId = user.UserId;
                    workshifts.Add(userworkshift);
                }
                context.Userworkshifts.AddRange(workshifts);
                context.SaveChanges();

                var box = MessageBoxManager
                .GetMessageBoxStandard("Успех",
                    "Смена создана",
                    ButtonEnum.Ok);

                await box.ShowAsync();
            }
        }

        private void DeleteUserInShift()
        {
            List<User> newListAllUsers = new List<User>();

            if (Users != null)
            {
                newListAllUsers = Users.ToList();
            }

            newListAllUsers.Add(SelectedFromSelectedUsers);
            Users = new ObservableCollection<User>(newListAllUsers);

            var newListSelectedUsers = SelectedUsers.ToList().Where(x => x.UserId != SelectedFromSelectedUsers.UserId);
            SelectedUsers = new ObservableCollection<User>(newListSelectedUsers);
        }

        private void AddUserInShift()
        {
            List<User> newListSelectedUsers = new List<User>();

            if(SelectedUsers != null)
            {
                newListSelectedUsers = SelectedUsers.ToList();
            }

            newListSelectedUsers.Add(SelectedFromAllUsers);

            var listOfAllUsers = DatabaseHelper.refreshEntity<User>().ToList();

            List<User> newListAllUser = new();

            foreach (var user in listOfAllUsers)
            {
                bool userIsNotExist = true;

                foreach(var selectedUser in newListSelectedUsers)
                {
                    if(user.UserId == selectedUser.UserId)
                    {
                        userIsNotExist = false;
                    }
                }

                if (userIsNotExist)
                {
                    newListAllUser.Add(user);
                }
            }

            Users = new ObservableCollection<User>(newListAllUser);
            SelectedUsers = new ObservableCollection<User>(newListSelectedUsers);
        }

        private async Task<bool> CheckDate()
        {
            DateTimeOffset lastDate = new DateTimeOffset(new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day + 5));

            if (SelectedDate == DateTimeOffset.MinValue)
            {
                return false;
            }else if(SelectedDate < currentDateTime.ToDateTimeUnspecified())
            {
                var box = MessageBoxManager
             .GetMessageBoxStandard("Выбор даты",
             "Дата не должна быть позже сегоднешней даты",
                ButtonEnum.Ok);

                box.ShowAsync();

                return false;
            }
            else if(SelectedDate > lastDate)
            {
                SelectedDate = lastDate;

                var box = MessageBoxManager
               .GetMessageBoxStandard("Выбор даты",
               "Граница даты: " + lastDate,
                  ButtonEnum.Ok);

                box.ShowAsync();

                return false;
            }
            return true;
        }

    }
}
