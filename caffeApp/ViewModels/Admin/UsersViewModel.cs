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
using Microsoft.EntityFrameworkCore;
using Avalonia.Media.Imaging;
using System.IO;

namespace caffeApp.ViewModels.Admin
{
    public class UsersViewModel : ViewModelBase, IActivatableViewModel
    {
        private ObservableCollection<User> _users;

        private ObservableCollection<Role> _roles;

        public override string? UrlPathSegment { get; set; }

        public override ViewModelActivator Activator { get; set; }

        public override IScreen HostScreen { get; set; }

        private Bitmap _imageUser;

        private Bitmap _imageAggreemnt;

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
                var listOfUsers = DbContextProvider
                .GetContext()
                .Users
                .Include(u => u.Role)
                .Include(u => u.Document)
                .ToList();
       
                Users = new ObservableCollection<User>(listOfUsers);
     
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
                FullName = selectedUser.getFullName();
                UserRole = selectedUser.Role ?? new Role { Name = "не указано" };
                if(selectedUser.Document == null) {
                    selectedUser.Document = new Document { Contractlink = " ", Photolink = " " };
                }
                try
                {
                    ImageUser = new Bitmap(Path.Combine(selectedUser.Document.Photolink));
                    ImageAggreemnt = new Bitmap(Path.Combine(selectedUser.Document.Contractlink));
                }
                catch (Exception ex)
                {
                    ImageUser = null;
                    ImageAggreemnt = null;
                    Console.WriteLine(ex.Message);
                }
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

        public Bitmap? ImageUser { 
            get => _imageUser;
            set => this.RaiseAndSetIfChanged(ref _imageUser, value);
        }
        public Bitmap? ImageAggreemnt { 
            get => _imageAggreemnt; 
            set => this.RaiseAndSetIfChanged(ref _imageAggreemnt, value); 
        }
    }
}
