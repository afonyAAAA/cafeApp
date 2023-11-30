using Avalonia;
using Avalonia.Markup.Xaml;
using caffeApp.ViewModels.Admin;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace caffeApp.ViewModels;


public class MainViewModel : ReactiveObject, IScreen
{
    // The Router associated with this Screen.
    // Required by the IScreen interface.

    public RoutingState Router { get; } = new RoutingState();

    // The command that navigates a user to first view model.
    public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }

    // The command that navigates a user back.
    public ReactiveCommand<Unit, IRoutableViewModel> GoBack => Router.NavigateBack;

    public bool _adminFunctionalIsOpen;
    public bool _sheffFunctionalIsOpen;
    public bool _waiterFunctionalIsOpen;
    public bool _isNotAuthorizedUser;

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

    public MainViewModel()
    {
        _adminFunctionalIsOpen = false;
        _sheffFunctionalIsOpen = false;
        _waiterFunctionalIsOpen = false;

        _isNotAuthorizedUser = true;

        if (_isNotAuthorizedUser)
        {
            Router.Navigate.Execute(new AuthorizationViewModel(this));
        }

        GoNext = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new UsersViewModel(this))
        );
    }
}
