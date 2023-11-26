using Avalonia.Controls;
using caffeApp.models;
using caffeApp.Sources;
using caffeApp.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

namespace caffeApp.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private ObservableCollection<User> _users;

        private string _fullName;

        private User _selectedUser;

        private AdminView _adminView;

        public AdminViewModel(AdminView adminView)
        {
            _adminView = adminView;
            _fullName = string.Empty;
            var listOfUsers = DbContextProvider.GetContext().Users.ToList();
            Users = new ObservableCollection<User>(listOfUsers);

            this.WhenAnyValue(vm => vm).Subscribe(_ => { { UpdateFullName(); }; }) ;
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

        private void UpdateFullName()
        {
            FullName = _selectedUser.getFullName();
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
