﻿using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Media.Imaging;
using System.Reactive.Linq;
using System.IO;
using caffeApp.Sources;
using caffeApp.Desktop;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Collections.ObjectModel;

namespace caffeApp.ViewModels.Admin
{
    public class RegistrationViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get; set; }
        public override IScreen HostScreen { get; set; }
        public override ViewModelActivator Activator { get; set; }

        private ObservableCollection<Role> _roles;
        private ObservableCollection<User> _users;
        private Role _role;
        private string _firstName;
        private string _secondName;
        private string _surname;
        private string _login;
        private string _password;
        private string _selectedImageUserPath;
        private string _selectedImageAgreementPath;
        private Bitmap _userImageBitmap;
        private Bitmap _aggrementImageBitmap;

        readonly string pathUsersFolder = Path.Combine(Environment.CurrentDirectory + "\\" + "Users");


        public ReactiveCommand<Unit, Unit> OpenDialogFileUserImage { get; }

        public ReactiveCommand<Unit, Unit> OpenDialogFileAgreementImage { get; }

        public ReactiveCommand<Unit, Unit> SignUpCommand { get; }


        public Bitmap UserImageBitmap
        {
            get => _userImageBitmap;
            set => this.RaiseAndSetIfChanged(ref _userImageBitmap, value);
        }

        public Bitmap AggrementImageBitmap {
            get => _aggrementImageBitmap;
            set => this.RaiseAndSetIfChanged(ref _aggrementImageBitmap, value);
        }

        public string FirstName {
            get => _firstName; 
            set
            {
                if(value.Length < 30)
                   this.RaiseAndSetIfChanged(ref _firstName, value);
                
            }
        }

        public string SecondName { 
            get => _secondName;
            set {
                if (value.Length < 30)
                    this.RaiseAndSetIfChanged(ref _secondName, value);
            }
        }

        public string Surname { 
            get => _surname;
            set {
                if (value.Length < 30)
                    this.RaiseAndSetIfChanged(ref _surname, value);
            }
        }

        public string Login { 
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }

        public string Password {
            get => _password; 
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public string SelectedImageUserPath {
            get => _selectedImageUserPath;
            set => this.RaiseAndSetIfChanged(ref _selectedImageUserPath, value);
        }

        public string SelectedImageAgreementPath
        {
            get => _selectedImageAgreementPath;
            set => this.RaiseAndSetIfChanged(ref _selectedImageAgreementPath, value);
        }

        public Role Role {
            get => _role;
            set => this.RaiseAndSetIfChanged(ref _role, value);
        }

        public ObservableCollection<Role> Roles {
            get => _roles; 
            set => this.RaiseAndSetIfChanged(ref _roles, value); 
        }

        public ObservableCollection<User> Users { 
            get => _users;
            set => this.RaiseAndSetIfChanged(ref _users, value);
        }
     
        [Obsolete]
        public RegistrationViewModel(IScreen screen)
        {
            Activator = new ViewModelActivator();

            HostScreen = screen;

            OpenDialogFileUserImage = ReactiveCommand.CreateFromTask(async () =>
            {
                SelectedImageUserPath = await openFilePicker();
            });

            OpenDialogFileAgreementImage = ReactiveCommand.CreateFromTask(async () =>
            {
                SelectedImageAgreementPath = await openFilePicker();
            });

            SignUpCommand = ReactiveCommand.Create(() =>
            {
                SignUp();
            });

            this.WhenAnyValue(x => x.SelectedImageUserPath).Subscribe(tuple =>
            {
                if(tuple != null)
                {
                    UserImageBitmap = new Bitmap(tuple);
                }
            });

            this.WhenAnyValue(x => x.SelectedImageAgreementPath).Subscribe(tuple =>
            {
                if(tuple != null)
                {
                    AggrementImageBitmap = new Bitmap(tuple);
                }
            });

            this.WhenActivated(disposables => {
              
                var roles = DbContextProvider.GetContext().Roles.ToList();
                var users = DbContextProvider.GetContext().Users.ToList();

                Roles = new(roles);
                Users = new(users);

                /* handle activation */
                Disposable
                    .Create(() =>
                    {
                    })
                    .DisposeWith(disposables);
            });
        }

        [Obsolete]
        private async Task<string> openFilePicker()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите изображение",
                Filters = GetImageFileFilters().ToList()
            };

            var result = await dialog.ShowAsync(GetWindow());

            string path = null;

            if (result != null && result.Any())
            {
               path = result.First();
            }
           
            return path;
        }

        [Obsolete]
        private FileDialogFilter[] GetImageFileFilters()
        {
            // Фильтры для изображений
            return new[]
            {
                new FileDialogFilter
                {
                    Name = "Изображения",
                    Extensions = new List<string> { "jpg", "jpeg", "png", "gif", "bmp" }
                },
                new FileDialogFilter
                {
                    Name = "Все файлы",
                    Extensions = new List<string> { "*" }
                }
            };
        }

        private async void SignUp()
        {
            if (!isValidLogin())
            {
                var box = MessageBoxManager
                       .GetMessageBoxStandard("Регистрация", 
                       "Минимальная длина логина 11 символов",
                    ButtonEnum.Ok);

                var result = await box.ShowAsync();
            }
            else if(!isValidPassword())
            {
                var box = MessageBoxManager
                   .GetMessageBoxStandard("Регистрация",
                   "Минимальная длина пароля 13 символов",
                ButtonEnum.Ok);

                var result = await box.ShowAsync();
            }
            else if (isValidName(FirstName) && isValidName(SecondName) && isValidName(Surname))
            {
                var box = MessageBoxManager
                  .GetMessageBoxStandard("Регистрация",
                  "Все части имени должны быть заполнены",
                ButtonEnum.Ok);

                var result = await box.ShowAsync();
            }
            else if(Role.RoleId == 0)
            {

                var box = MessageBoxManager
                  .GetMessageBoxStandard("Регистрация",
                  "Роль должна быть заполнена",
                ButtonEnum.Ok);

                var result = await box.ShowAsync();
            }
            else if (Users.ToList().Find(x => x.Login == Login) != null)
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Регистрация",
                    "Такой логин уже занят",
                ButtonEnum.Ok);

                var result = await box.ShowAsync();
            }
            else
            {
                try
                {
                    var context = DbContextProvider.GetContext();
                    
                    User user = new();
                    user.Login = Login;
                    user.Password = Password;
                    user.FirstName = FirstName;
                    user.SecondName = SecondName;
                    user.Surname = Surname;
                    user.RoleId = Role.RoleId;

                    var createdUser = context.Users.Add(user);
                    context.SaveChanges();

                    StringBuilder sb = new();

                    sb.Append(pathUsersFolder);
                    sb.Append("\\" + FirstName + SecondName.First().ToString() + Surname.First().ToString());
                    sb.Append("-" + createdUser.Entity.UserId);

                    var pathFolderUser = sb.ToString();

                    createUsersDirectory();
                    createUserDirectory(pathFolderUser);
                    File.SetAttributes(pathFolderUser, FileAttributes.Normal);

                    var documentId = await createDocumentUser(pathFolderUser, context);

                    var fileNameUserPhoto = Path.GetFileName(SelectedImageUserPath);
                    var fileNameAggreementPhoto = Path.GetFileName(SelectedImageAgreementPath);

                    File.Copy(SelectedImageUserPath, Path.Combine(pathFolderUser, fileNameUserPhoto), true);
                    File.Copy(SelectedImageAgreementPath, Path.Combine(pathFolderUser, fileNameAggreementPhoto), true);

                    user.DocumentId = documentId;
                    context.Update(user);
                    context.SaveChanges();

                    await HostScreen.Router.Navigate.Execute(new UsersViewModel(HostScreen));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
        }

        private void createUsersDirectory()
        {
            if (!Directory.Exists(pathUsersFolder))
            {
                Directory.CreateDirectory(pathUsersFolder);
            }

        }

        private async Task<int> createDocumentUser(string pathFolderUser, CafeContext context)
        {
            var fileNameUserPhoto = Path.GetFileName(SelectedImageUserPath);
            var fileNameAggreementPhoto = Path.GetFileName(SelectedImageAgreementPath);
            Desktop.Document document = new Desktop.Document();
            document.Contractlink = Path.Combine(pathFolderUser, fileNameAggreementPhoto);
            document.Photolink = Path.Combine(pathFolderUser, fileNameUserPhoto);
            var documentid = context.Documents.Add(document);
            context.SaveChanges();
            return documentid.Entity.DocumentId;
        }

        private void createUserDirectory(string pathFolderUser)
        {
            if (!Directory.Exists(pathFolderUser))
            {
                Directory.CreateDirectory(pathFolderUser).Attributes = FileAttributes.Normal;
            }
        }

        private Window GetWindow()
        {
            return new Window();
        }

        private bool isValidPassword()
        {
            if(!string.IsNullOrWhiteSpace(Password) && Password.Length < 12)
            {
                return false;
            }
            return true;
        }
        private bool isValidLogin()
        {
            if (!string.IsNullOrWhiteSpace(Login) && Login.Length < 6)
            {
                return false;
            }
            return true;
        }
        private bool isValidName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            return true;
        }

    }
}
