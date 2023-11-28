using caffeApp.models;
using caffeApp.Sources;
using caffeApp.Views;
using System.Reactive.Linq;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using Avalonia.Platform;
using Avalonia.Controls;
using System.Reactive.Disposables;

namespace caffeApp.ViewModels
{
    public class AdminViewModel : ViewModelBase, IRoutableViewModel, IActivatableViewModel
    {
        private ObservableCollection<User> _users;

        public Grid GridSelectedUser = new Grid();

        private ObservableCollection<Role> _roles;

        public ViewModelActivator Activator { get; }

        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

        private string _fullName; 

        private User _selectedUser;

        private Role _userRole;

        private AdminView _adminView;

        public AdminViewModel(AdminView adminView, IScreen screen)
        {
            Activator = new ViewModelActivator();

            _adminView = adminView;
            _fullName = string.Empty;
            var listOfUsers = DbContextProvider.GetContext().Users.ToList();
            var listOfRoles = DbContextProvider.GetContext().Roles.ToList();
            Users = new ObservableCollection<User>(listOfUsers);
            Roles = new ObservableCollection<Role>(listOfRoles);

            this.WhenAnyValue(vm => vm.SelectedUser).Subscribe(UpdateInfoUser);

            HostScreen = screen;

            GridSelectedUser.IsVisible = true;

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                /* handle activation */
                Disposable
                    .Create(() => {
                    })
                    .DisposeWith(disposables);
            });

        }

        public ObservableCollection<Role> Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _roles, value);
            }
        }

        public ObservableCollection<User> Users
        {
            get
            {
                return _users;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _users, value);
            }
        }

        private void UpdateInfoUser(User selectedUser)
        {
            if(selectedUser != null)
            {
                var role = _roles.First(role => role.RoleId == selectedUser.RoleId);

                FullName = selectedUser.getFullName();
                UserRole = role;

                _adminView.grid_selected_user.IsVisible = true;
            }
        }

        public string FullName
        {
            get
            {
                 return _fullName;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _fullName, value);
            }
        }

        public Role UserRole
        {
            get
            {
                return _userRole;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _userRole, value);
            }
        }

        public User SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedUser, value);
            }
        }
    }

}
