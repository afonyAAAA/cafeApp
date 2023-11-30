using caffeApp.Sources;
using DynamicData.Binding;
using Npgsql.Replication;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ReactiveCommand<Unit, Unit> SubmitCommand { get; set; }

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

                SubmitCommand = ReactiveCommand.Create(() =>
                {
                    LogIn();
                }, inputIsValid);

                /* handle activation */
                Disposable
                    .Create(() =>
                    {
                    })
                    .DisposeWith(disposables);
            });
        }

        private void LogIn()
        {
            var users = DbContextProvider.GetContext().Users.ToList();
            var user = users.Find(x => x.FirstName == ""); 
            
        }

        private bool checkInputValidForLogin(string login)
        {
            return !string.IsNullOrWhiteSpace(login) && login.Length > 7;
        }

        private bool checkInputValidForPassword(string login)
        {
            return !string.IsNullOrWhiteSpace(login) && login.Length > 10;
        }
    }
}
