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
        public override string? UrlPathSegment { get; set; }
        public override IScreen HostScreen { get; set; }
        public override ViewModelActivator Activator { get; set; }

        private DateTimeOffset _selectedDate;
        private TimeSpan _selectedTimeStart;
        private TimeSpan _selectedTimeEnd;
        private IObservable<bool> _deleteIsClickable;
        private IObservable<bool> _addIsClickable;
        private ObservableCollection<User> _users;
        private ObservableCollection<Workshift> _workShifts;
        private ObservableCollection<User> _selectedUsers;
        private ObservableCollection<Place> _places;
        private List<(int?, User)> _userPlaces;
        private User _selectedFromSelectedUsers;
        private User _selectedFromFromAllUsers;
        private Place _selectedPlace;
        private Place _selectedPlaceUser;
        private bool _userFromAllUsersSelected;

        public ReactiveCommand<Unit, Unit> AddUserShift { get; }
        public ReactiveCommand<Unit, Unit> DeleteUserShift { get; }
        public ReactiveCommand<Unit, Unit> SavePlace { get; }
        public ReactiveCommand<Unit, Unit> Submit { get; }

        public ObservableCollection<User> Users {
            get
            { 
                var usersIsNotFired = _users.Where(x =>!x.IsFired).ToList();

                return new(usersIsNotFired);
            }
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

        public User SelectedFromSelectedUsers
        { 
            get => _selectedFromSelectedUsers;
            set => this.RaiseAndSetIfChanged(ref _selectedFromSelectedUsers, value);
        }

        public User SelectedFromAllUsers
        {
            get => _selectedFromFromAllUsers;
            set => this.RaiseAndSetIfChanged(ref _selectedFromFromAllUsers, value);
        }

        public IObservable<bool> DeleteIsClickable {
            get => _deleteIsClickable;
            set => this.RaiseAndSetIfChanged(ref _deleteIsClickable, value);
        }

        public IObservable<bool> AddIsClickable { 
            get => _addIsClickable; 
            set => this.RaiseAndSetIfChanged(ref _addIsClickable, value);
        }

        public ObservableCollection<Place> Places { 
            get => _places; 
            set => this.RaiseAndSetIfChanged(ref _places, value);
        }
        public bool UserFromAllUsersSelected { 
            get => _userFromAllUsersSelected;
            set => this.RaiseAndSetIfChanged(ref _userFromAllUsersSelected, value);
        }
        public List<(int?, User)> UserPlaces { 
            get => _userPlaces; 
            set => this.RaiseAndSetIfChanged(ref _userPlaces, value);
        }
        public Place SelectedPlace { 
            get => _selectedPlace; 
            set => this.RaiseAndSetIfChanged(ref _selectedPlace, value);
        }
        public Place SelectedPlaceUser {
            get => _selectedPlaceUser;
            set => this.RaiseAndSetIfChanged(ref _selectedPlaceUser, value);
        }

        public AddWorkShiftViewModel(IScreen screen) {

            Activator = new ViewModelActivator();

            HostScreen = screen;

            UserFromAllUsersSelected = false;

            UserPlaces = new();

            Places = DatabaseHelper.refreshEntity<Place>();

            var usersWithoutAdmins = DatabaseHelper.refreshEntity<User>(x => x.Role).Where(x => x.RoleId != 3).ToList();
                
            Users = new(usersWithoutAdmins);

            WorkShifts = DatabaseHelper.refreshEntity<Workshift>();

            SelectedPlace = new();

            SelectedUsers = new();

            SelectedFromAllUsers = new();

            SelectedFromSelectedUsers = new();

            DateTime currentDateTime = DateTime.Now;

            SelectedDate = new DateTimeOffset(new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day));

            this.WhenAnyValue(x => x.SelectedFromSelectedUsers).WhereNotNull().Subscribe(selectedUser => {

                UserFromAllUsersSelected = true;

                var currentUserPlace = UserPlaces.FirstOrDefault(x => x.Item2.UserId == selectedUser.UserId);

                if(currentUserPlace.Item1 == null)
                {
                    SelectedPlaceUser = new();
                }
                else
                {
                    SelectedPlaceUser = Places.FirstOrDefault(x => x.PlaceId == currentUserPlace.Item1);
                }

            });

            DeleteIsClickable = this.WhenAnyValue(x => x.SelectedFromSelectedUsers)
        .Select(selectedUser => selectedUser != null && Users.ToList().All(x => x.UserId != selectedUser.UserId));

            AddIsClickable = this.WhenAnyValue(x => x.SelectedFromAllUsers)
                .Select(selectedUser => selectedUser != null && SelectedUsers.ToList().All(x => x.UserId != selectedUser.UserId))
                .StartWith(true);


            SavePlace = ReactiveCommand.CreateFromTask(async () => { 
                if(SelectedFromSelectedUsers == null)
                {
                    var box = MessageBoxManager
                    .GetMessageBoxStandard("Сохранение места",
                    "Выберите официанта",
                     ButtonEnum.Ok);

                    box.ShowAsync();
                }
                else if (SelectedFromSelectedUsers.RoleId != 1)
                {
                    var box = MessageBoxManager
                    .GetMessageBoxStandard("Сохранение места",
                    "Выберите пользователя с ролью \"официант\" ",
                    ButtonEnum.Ok);

                    box.ShowAsync();
                }
                else if (SelectedPlace.PlaceId == 0)
                {
                    var box = MessageBoxManager
                    .GetMessageBoxStandard("Сохранение места",
                    "Выберите место обслуживания",
                    ButtonEnum.Ok);

                    box.ShowAsync();
                }
                else
                {
                    var userPlace = (SelectedPlace.PlaceId, SelectedFromSelectedUsers);

                    var userIsFound = false;

                    var index = 0;

                    foreach(var userPlaceItem in UserPlaces)
                    {
                        if (userPlaceItem.Item2.UserId == userPlace.SelectedFromSelectedUsers.UserId)
                        {
                            userIsFound = true;
                            break;
                        }
                        ++index;
                    }

                    if (!userIsFound)
                    {
                        index = -1;
                    }

                    if (index == -1)
                    {
                        UserPlaces.Add(userPlace);
                    }
                    else
                    {
                        UserPlaces[index] = userPlace;
                    }

                    SelectedPlaceUser = Places.First(x => x.PlaceId == userPlace.PlaceId);
                    
                }
            });

            Submit = ReactiveCommand.Create(() =>
            {
                if (CheckDate().Result)
                {
                    var result = CreateWorkShift();

                    if (result.Result)
                    {
                        HostScreen.Router.Navigate.Execute(new ShiftViewModel(HostScreen));
                    }
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

        private async Task<bool> CreateWorkShift()
        {
            if (SelectedUsers.Count < 4)
            {
                var box = MessageBoxManager
              .GetMessageBoxStandard("Создание смены",
                  "В смене не может быть меньше 4 человек",
                  ButtonEnum.Ok);

                box.ShowAsync();

                return false;
            }
            else if (SelectedUsers.Count > 7)
            {
                var box = MessageBoxManager
           .GetMessageBoxStandard("Создание смены",
               "В смене не может быть больше 7 человек",
               ButtonEnum.Ok);

                box.ShowAsync();

                return false;
            }
            else
            {
                var context = DbContextProvider.GetContext();
                Workshift workshift = new();
                workshift.Date = new DateOnly(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day);
                workshift.Timestart = new TimeOnly(SelectedTimeStart.Hours, SelectedTimeStart.Minutes);
                workshift.Timeend = new TimeOnly(SelectedTimeEnd.Hours, SelectedTimeEnd.Minutes);
                var createdWorkShift = context.Workshifts.Add(workshift);
                context.SaveChanges();

                List<Userworkshift> workshifts = new List<Userworkshift>();
                foreach(var user in SelectedUsers)
                {
                    var placeId = UserPlaces.FirstOrDefault(x => x.Item2.UserId == user.UserId).Item1;


                    Userworkshift userworkshift = new Userworkshift();
                    userworkshift.WorkshiftId = createdWorkShift.Entity.WorkshiftId;
                    userworkshift.UserId = user.UserId;
                    userworkshift.PlaceId = placeId;

                    workshifts.Add(userworkshift);
                }
                context.Userworkshifts.AddRange(workshifts);
                context.SaveChanges();

                var box = MessageBoxManager
                .GetMessageBoxStandard("Создание смены",
                    "Смена создана",
                    ButtonEnum.Ok);

                box.ShowAsync();

                return true;
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
            SelectedFromSelectedUsers = new();
            UserPlaces = UserPlaces.Where(x => x.Item2.UserId != SelectedFromSelectedUsers.UserId).ToList();
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

            SelectedFromAllUsers = new();
            Users = new ObservableCollection<User>(newListAllUser);
            SelectedUsers = new ObservableCollection<User>(newListSelectedUsers);
            SelectedPlaceUser = new();
        }

        private async Task<bool> CheckDate()
        {
            DateTime currentDateTime = DateTime.Now;
            DateTimeOffset lastDate = new DateTimeOffset(new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day + 5));

            if (SelectedDate == DateTimeOffset.MinValue)
            {
                var box = MessageBoxManager
                .GetMessageBoxStandard("Выбор даты",
                "Выберите корректную дату",
                ButtonEnum.Ok);

                box.ShowAsync();

                return false;
            }
            else if(new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, SelectedTimeEnd.Hours, SelectedTimeEnd.Minutes, 0) < currentDateTime)
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
