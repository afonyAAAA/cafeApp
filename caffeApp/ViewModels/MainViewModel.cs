using Avalonia;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace caffeApp.ViewModels;

public class MainViewModel : ViewModelBase, IScreen
{
    // The Router associated with this Screen.
    // Required by the IScreen interface.
    public RoutingState Router { get; } = new RoutingState();

    // The command that navigates a user to first view model.
    public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }

    // The command that navigates a user back.
    public ReactiveCommand<Unit, IRoutableViewModel> GoBack => Router.NavigateBack;

    public MainViewModel()
    {
        // Manage the routing state. Use the Router.Navigate.Execute
        // command to navigate to different view models. 
        //
        // Note, that the Navigate.Execute method accepts an instance 
        // of a view model, this allows you to pass parameters to 
        // your view models, or to reuse existing view models.
        //

  
    

        GoNext = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new AdminViewModel(new Views.AdminView(), this))
        );
    }
}
