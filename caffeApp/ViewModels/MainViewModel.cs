using Avalonia;
using Avalonia.Markup.Xaml;
using caffeApp.Desktop;
using caffeApp.models;
using caffeApp.ViewModels.Admin;
using DynamicData.Binding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace caffeApp.ViewModels;


public class MainViewModel : ReactiveObject, IScreen, IActivatableViewModel, IAuthorizationCallback
{
 
    public RoutingState Router { get; } = new RoutingState();
    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    public ReactiveCommand<Unit, IRoutableViewModel> OpenUserView { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> OpenShiftView { get; }
    public ReactiveCommand<Unit, Unit> LogOut { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoBack => Router.NavigateBack;

    private User _user;
    private bool _adminFunctionalIsOpen;
    private bool _sheffFunctionalIsOpen;
    private bool _waiterFunctionalIsOpen;
    private bool _isNotAuthorizedUser;

    public User User
    {
        get
        {
            return _user;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref _user, value);
        }
    }

    public bool AdminFunctionalIsOpen
    {
        get
        {
            return _adminFunctionalIsOpen;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref _adminFunctionalIsOpen, value);
        }
    }

    public bool SheffFunctionalIsOpen
    {
        get
        {
            return _sheffFunctionalIsOpen;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref _sheffFunctionalIsOpen, value);
        }
    }

    public bool WaiterFunctionalIsOpen
    {
        get
        {
            return _waiterFunctionalIsOpen;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref _waiterFunctionalIsOpen, value);
        }
    }

    public bool IsNotAuthorizedUser
    {
        get
        {
            return _isNotAuthorizedUser;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref _isNotAuthorizedUser, value);
        }
    }

    public MainViewModel()
    {
        _adminFunctionalIsOpen = false;
        _sheffFunctionalIsOpen = false;
        _waiterFunctionalIsOpen = false;
        _isNotAuthorizedUser = true;

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(x => x.IsNotAuthorizedUser).Subscribe(tuple =>
            {
                var user = UserHelper.getAuthorizedUserInfo();

                if (user.UserId != 0)
                {
                    IsNotAuthorizedUser = false;
                    switch (user.RoleId)
                    {
                        case 1:
                            WaiterFunctionalIsOpen = true;
                            break;
                        case 2:
                            SheffFunctionalIsOpen = true;
                            break;
                        case 3:
                            AdminFunctionalIsOpen = true;
                            break;
                    }
                    Router.NavigationStack.Clear();
                }
                else
                {
                    Router.Navigate.Execute(new AuthorizationViewModel(this, this));
                }
            }).DisposeWith(disposables);
        });

        OpenShiftView = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new ShiftViewModel(this))
        );

        OpenUserView = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new UsersViewModel(this))
        );

        LogOut = ReactiveCommand.Create(() => {
            logOut();
            Router.Navigate.Execute(new AuthorizationViewModel(this, this));
        });
    }

    private void logOut()
    {
        string pathToFile = Path.Combine(Environment.CurrentDirectory, "UserData.json");
        File.Delete(pathToFile);
        User = null;
        IsNotAuthorizedUser = true;
        AdminFunctionalIsOpen = false;
        SheffFunctionalIsOpen = false;
        WaiterFunctionalIsOpen = false;
    }

    public void OnAuthorizationComplete()
    {
        IsNotAuthorizedUser = false;
    }
}
