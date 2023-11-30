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

namespace caffeApp.ViewModels
{
    public class AuthorizationViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override IScreen HostScreen { get; set; }
        public override ViewModelActivator Activator { get; set; }

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

        public AuthorizationViewModel(IScreen screen) {

            Activator = new ViewModelActivator();

         

            this.WhenActivated(disposables => {

                IObservable<bool> loginIsValid = this.WhenAnyValue(
                x => x.Login,
                x => checkInputValidForLogin(x)
                );

                IObservable<bool> passwordIsValid = this.WhenAnyValue(
                x => x.Password,
                x => checkInputValidForPassword(x)
                );

                var inputIsValid = Observable.CombineLatest(passwordIsValid, loginIsValid, (passwordValid, loginValid) => passwordValid && loginValid)
                      .Take(1);

                SubmitCommand = ReactiveCommand.Create(() => LogInAsync());
                

                /* handle activation */
                Disposable
                    .Create(() =>
                    {
                    })
                    .DisposeWith(disposables);
            });
        }

        public ReactiveCommand<Unit, Unit> SubmitCommand { get; set; }

        private async Task LogInAsync()
        {
            var users = DbContextProvider.GetContext().Users.ToList();
            var userIsExist = users.Find(x => x.Login == Login) != null;
            var user = users.Find(x => x.Login == Login && x.Password == Password); 
            
            if(user != null)
            {
                saveUserInSystem(user);
            }
            else
            {
                if (userIsExist)
                {
                    var box = MessageBoxManager
                            .GetMessageBoxStandard("Авторизация", "Неверный логин или пароль",
                         ButtonEnum.YesNo);

                    var result = await box.ShowAsync();
                }
                else
                {
                    var box = MessageBoxManager
                            .GetMessageBoxStandard("Авторизация", "Пользователя с таким логином не существует",
                         ButtonEnum.YesNo);

                    var result = await box.ShowAsync();
                }
            
            }
        }

        private void saveUserInSystem(User user)
        {
            // Преобразуем объект пользователя в JSON
            string json = JsonConvert.SerializeObject(user);

            // Определение пути к файлу
            string fileName = "UserData.json";
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            // Записываем JSON в файл
            File.WriteAllText(filePath, json);
        }

        private bool checkInputValidForLogin(string login)
        {
            return !string.IsNullOrWhiteSpace(login) && login.Length > 1;
        }

        private bool checkInputValidForPassword(string login)
        {
            return !string.IsNullOrWhiteSpace(login) && login.Length > 1;
        }
    }
}
