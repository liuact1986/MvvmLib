## MvvmLib.Windows (Navigation) [uwp]

* **ViewModelLocator**: allows to **resolve ViewModel** for views
* **NavigationManager**: allows to create and manage **navigation services**
* **FrameNavigationService**: allows to **navigate**, go back, go forward, **cancel navigation** and **notify viewmodel**
* **INavigatable**: allows the view models to be notified on navigate
* **IDeactivatable**: allows to **cancel** navigation
* **ILoadedEventListener**: allows to be notified from view model when the **view** is **loaded**
* **BackRequestManager**: allows to show the **back button** in **title bar**
* **BootstrapperBase**: bootstrapper base class


## ViewModelLocator

> Allows to resolve automatically the ViewModel for the view with **ResolveViewModel** Attached Property.

Default **convention**:

* Views in `Views` namespace
* View models in `ViewModels` namespace
* View model name: _view name + "ViewModel"_ (example: MainPageViewModel for MainPage)

On each view that requires to resolve the view model:

```xml
<Page ...
    xmlns:nav="using:MvvmLib.Navigation"
    nav:ViewModelLocator.ResolveViewModel="True">
```

### Change the default convention

Example with "View" and "ViewModel" directories/namespaces

(App)
```cs
ViewModelLocationProvider.SetViewTypeToViewModelTypeResolver((viewType) =>
        {
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

            var viewName = viewType.FullName;
            viewName = viewName.Replace(".View.", ".ViewModel."); // <=
            var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}ViewModel, {1}", viewName, viewAssemblyName);

            return Type.GetType(viewModelName);
        });
```

## NavigationManager

> Allows to register Navigation services.

**Static methods**:

* **Register**: register a frame, with a key if the application have more than one frame
* **IsRegistered**
* **Unregister**

(App)

```cs
NavigationManager.Register(rootFrame);
```

_Tip_ : x:FieldModifier="Public" allows to access to a frame in a page. For example register a frame for a page with a NavigationView or a SplitView.
```cs
var frame = ((Shell)shell).MainFrame; //  x:FieldModifier="Public"
NavigationManager.Register(frame);
```

Use a Bootstrapper. Sample Create a Boostrapper with MvvmLib IoC Container.

We could use any IoC container (Unity, StructureMap, etc.)

```cs
public abstract class MvvmBootstrapper : BootstrapperBase
{
    protected IInjector container;

    public MvvmBootstrapper(IInjector container)
    {
        this.container = container;
    }

    protected override void SetViewModelFactory()
    {
        ViewModelLocationProvider.SetViewModelFactory((viewModelType) => container.GetInstance(viewModelType));
    }

    protected override void RegisterRequiredTypes()
    {
        container.RegisterType<INavigationManager, NavigationManager>();
        container.RegisterType<IBackRequestManager, BackRequestManager>();
    }
}
```

... The create a Bootstrapper class

```cs
public class Bootstrapper : MvvmBootstrapper
{
    public Bootstrapper(IInjector container)
        : base(container)
    { }

    protected override void RegisterTypes()
    {
        container.RegisterType<IMyService, MyService>();
        container.RegisterSingleton<PageBViewModel>();
    }

    protected override Page CreateShell()
    {
        return container.GetInstance<Shell>();
    }

    protected override void ConfigureNavigation(Page shell)
    {
        var frame = ((Shell)shell).MainFrame; //  x:FieldModifier="Public"
        NavigationManager.Register(frame);
    }

    protected override void InitializeShell(Page shell)
    {
        Window.Current.Content = shell;
    }
}
```

In App

```cs
sealed partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
        this.Suspending += OnSuspending;
    }

    private const string NavigationStateName = "__navigationstate__";

    public new static App Current => (App)Application.Current;

    public IInjector Container { get; private set; } = new Injector();

    public INavigationManager NavigationManager { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        // create and run a boostrapper with the ioc container
        var bootstrapper = new Bootstrapper(Container);
        bootstrapper.Run();

        NavigationManager = Container.GetInstance<INavigationManager>();

        // App lifeycle 
        if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(NavigationStateName))
            {
                var navigationState = ApplicationData.Current.LocalSettings.Values[NavigationStateName].ToString();
                NavigationManager.GetDefault().SetNavigationState(navigationState);
            }
        }
    }

    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
        var deferral = e.SuspendingOperation.GetDeferral();

        var navigationState = NavigationManager.GetDefault().GetNavigationState();
        ApplicationData.Current.LocalSettings.Values[NavigationStateName] = navigationState;

        deferral.Complete();
    }
}
```

**Methods**:

Use a IoC container to create and inject the service (INavigationmanager) in ViewModels/ views

Get The Navigation Service:

* **GetDefault**
* **GetNamed** for a frame registered with a key 

Sample:

```cs
await navigationManager.GetDefault().NavigateAsync(typeof(MainPage));
// with parameter
await navigationManager.GetDefault().NavigateAsync(typeof(MainPage), "My parameter");

// go back
await navigationManager.GetDefault().GoBackAsync();

// go forward
await navigationManager.GetDefault().GoForwardAsync();

// redirect
await navigationManager.GetDefault().RedirectAsync(typeof(LoginPage), "My parameter");
```

Navigation Service Methods:

| Method | Description |
| --- | --- |
| NavigateAsync | Allows to navigate to a new page |
| GoBackAsync | Allows to go to the previous page|
| GoForwardAsync | Allows to go the next page |
| RedirectAsync | Allows to redirect to a page and do not add/ oremove current page from history |
| GetNavigationState | Allows to get frame navigation state for app lifeycle |
| SetNavigationState | Allows to restore frame navigation state |

| Property | Description |
| --- | --- |
| BackStack | Frame Back stack |
| ForwardStack | Frame Forward stack |
| CanGoBack | Returns true if back stack history have more then one entry |
| CanGoForward | Returns true if forward stack history have one or more entries |


| Event | Description |
| --- | --- |
| CanGoBackChanged | Invoked after can go back changed |
| CanGoForwardChanged | Invoked after can go forward changed |
| Navigating | Invoked before navigation starts |
| Navigated | Invoked after navigation ends |
| NavigatingFailed | Invoked after navigation was cancelled |


## INavigatable

For View and/or View model

Example:

```cs
public class MainPageViewModel : INavigatable
{
    // Invoked after navigation ends
    public void OnNavigatedTo(object parameter)
    {
        
    }

    // Invoked before leaving
    public void OnNavigatingFrom()
    {
        
    }
}
```

## IDeactivatable Navigation Guard

> Allow to cancel navigation

Sample:

```cs
public class PageAViewModel :  IDeactivatable
{

    public async Task<bool> CanDeactivateAsync()
    {
        bool result = true;

        var dialog = new MessageDialog("Deactivate PageA?");
        dialog.Commands.Add(new UICommand("Ok", cmd => { }));
        dialog.Commands.Add(new UICommand("Cancel", cmd => { result = false; }));

        await dialog.ShowAsync();

        return result;
    }
}
```

MvvmLib.Windows has not IActivatable interface, because it's the frame that creates the view instance and we cannot access the the instance before navigating.

But its possible **cancel navigation** from view model **with redirectAsync** method. Example:

```cs
public class HomePageViewModel :  INavigatable
{
    private INavigationManager navigationManager;

    public HomePageViewModel(INavigationManager navigationManager)
    {
        this.navigationManager = navigationManager;
    }

    public async void OnNavigatedTo(object parameter, NavigationMode navigationMode)
    {
        // cancel navigation, remove the home page from history
        await navigationManager.GetDefault().RedirectAsync(typeof(LoginPage));
    }

    public void OnNavigatingFrom(bool isSuspending)
    {

    }
}
```

## BackRequestManager

> Allows to show/ hide back button in title bar and handle back button clicked.

```cs
var backRequestManager = new BackRequestManager(); // Or inject with a ioc container
backRequestManager.Handle(MainFrame, () => HandleBackRequested());
```

```cs
private async void HandleBackRequested()
{
    if (navigationService.CanGoBack)
    {
        await navigationService.GoBackAsync();
    }
}
```