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
using Avalonia;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using caffeApp.Desktop;
using System.Reactive;
using System.Diagnostics.Metrics;

namespace caffeApp.ViewModels.Admin
{
    public class UsersViewModel : ViewModelBase, IActivatableViewModel
    {
        private ObservableCollection<User> _users;

        private ObservableCollection<Role> _roles;

        public override string? UrlPathSegment { get; set; }

        public override ViewModelActivator Activator { get; set; }

        public override IScreen HostScreen { get; set; }

        private bool _selectedUserIsVisible;

        private string _fullName;

        private User _selectedUser;

        private Role _userRole;

        public ReactiveCommand<Unit, Unit> OpenRegistrationView { get; }

        public UsersViewModel(IScreen screen)
        {
            Activator = new ViewModelActivator();

            HostScreen = screen;

            OpenRegistrationView = ReactiveCommand.Create(() => {
                HostScreen.Router.Navigate.Execute(new RegistrationViewModel(HostScreen));
            });

            this.WhenActivated((disposables) =>
            {
                _selectedUserIsVisible = false;
                var listOfUsers = DbContextProvider.GetContext().Users.ToList();
                var listOfRoles = DbContextProvider.GetContext().Roles.ToList();
                Users = new ObservableCollection<User>(listOfUsers);
                Roles = new ObservableCollection<Role>(listOfRoles);
                this.WhenAnyValue(vm => vm.SelectedUser).Subscribe(UpdateInfoUser);

                /* handle activation */
                Disposable
                    .Create(() =>
                    {
                    })
                    .DisposeWith(disposables);
            });
        }

        public bool SelectedUserIsVisible
        {
            get
            {
                return _selectedUserIsVisible;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedUserIsVisible, value);
            }
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
            if (selectedUser != null)
            {
                var role = _roles.First(role => role.RoleId == selectedUser.RoleId);

                FullName = selectedUser.getFullName();
                UserRole = role;
                SelectedUserIsVisible = true;
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
