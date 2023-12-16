using caffeApp.Desktop;
using caffeApp.Sources;
using DynamicData.Binding;
using Newtonsoft.Json;
using Npgsql.Replication;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Windows.Input;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using caffeApp.models;

namespace caffeApp.ViewModels
{

    public interface IAuthorizationCallback
    {
        void OnAuthorizationComplete();
    }

    public class AuthorizationViewModel : ViewModelBase, IAuthorizationCallback
    {
        private IAuthorizationCallback authorizationCallback;

        public override string? UrlPathSegment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override IScreen HostScreen { get; set; }

        public override ViewModelActivator Activator { get; set; }

        private IObservable<bool> _inputIsValid;

        public string _login;

        public string _password;

        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _login, value);
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _password, value);
            }
        }

        public ReactiveCommand<Unit, Unit> SubmitCommand { get;}


        public AuthorizationViewModel(IScreen screen, IAuthorizationCallback callback) {

            HostScreen = screen;
            authorizationCallback = callback;
            Activator = new ViewModelActivator();

            var inputIsValid = this.WhenAnyValue(x => x.Login, x => x.Password)
                .Select(x => checkInputValidForLogin(x.Item1) && checkInputValidForPassword(x.Item2));

            SubmitCommand = ReactiveCommand.CreateFromTask(LogInAsync, inputIsValid);

            this.WhenActivated(disposables => {

                /* handle activation */
                Disposable
                    .Create(() =>
                    {
                    })
                    .DisposeWith(disposables);
            });
        }

        private async Task LogInAsync()
        {
            var users = DbContextProvider.GetContext().Users.ToList();
            var userIsExist = users.Find(x => x.Login == Login) != null;
            var user = users.Find(x => x.Login == Login && x.Password == Password);


            if(user != null)
            {

                user.Document = null;
                user.Role = null;
                saveUserInSystem(user);
                OnAuthorizationComplete();
            }
            else
            {
                if (userIsExist)
                {
                    var box = MessageBoxManager
                            .GetMessageBoxStandard("Авторизация", "Неверный логин или пароль",
                         ButtonEnum.Ok);

                    var result = await box.ShowAsync();
                }
                else
                {
                    var box = MessageBoxManager
                            .GetMessageBoxStandard("Авторизация", "Пользователя с таким логином не существует",
                         ButtonEnum.Ok);

                    var result = await box.ShowAsync();
                }
            
            }
        }

        private void saveUserInSystem(User user)
        {
            try
            {
                // Преобразуем объект пользователя в JSON
                string json = JsonConvert.SerializeObject(user);

                // Определение пути к файлу
                string fileName = "UserData.json";

                string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void OnAuthorizationComplete()
        {
            authorizationCallback.OnAuthorizationComplete();
        }

        private bool checkInputValidForLogin(string login)
        {
            return !string.IsNullOrWhiteSpace(login);
        }

        private bool checkInputValidForPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password);
        }
    }
}
