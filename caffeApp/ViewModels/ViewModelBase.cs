using ReactiveUI;

namespace caffeApp.ViewModels;

public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public abstract string? UrlPathSegment { get; set; }

    public abstract IScreen HostScreen { get; set; }

    public abstract ViewModelActivator Activator { get; set; }

}
