## MvvmLib.XF (Navigation) [Xamarin]
  
* **NavigationManager**: allows to create and manage **navigation services**
* **PageNavigationService**: allows to **push**, push modal, pop, pop modal, pop to root, handle system go back, **cancel navigation** and **notify viewmodel**
* **ViewModelLocator**: allows to **resolve ViewModel** for views
* **INavigatable**: allows the view models to be notified on navigate
* **IActivatable**, **IDeactivatable**: allow to cancel navigation
* **INavigationParameterKnowledge**: allows to store navigation parameter in the view model for system go back
* **IPageKnowledge**: allows to receive in view model the page
* **DialogService**: allows to display alerts and action sheets
* **BootstrapperBase**: bootstrapper base class
* **EventToCommandBehavior** and **BehaviorBase**

## ViewModelLocator

> Allows to resolve automatically the ViewModel for the view with **ResolveViewModel** Attached Property.

Default **convention**:

* Views in `Views` namespace
* View models in `ViewModels` namespace
* View model name: _view name + "ViewModel"_ (example: MainPageViewModel for MainPage)

(We can change this convention)

On each view that requires to resolve the view model:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage ...
             xmlns:nav="clr-namespace:MvvmLib.Navigation;assembly=MvvmLib.XF"
             nav:ViewModelLocator.ResolveViewModel="True">
```

## NavigationManager

> Allows to register Navigation services.

**Static methods**:

* **Register**: register a navigation page or a navigation service, with a key if the application require more than one service
* **IsRegistered**
* **Unregister**

Register from code behind for example

```cs
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MyTabbedPage : TabbedPage
{
    public MyTabbedPage()
    {
        InitializeComponent();

        var navigationService = NavigationManager.Register(tabNav, "tabNav");
    }
}
```

**Methods**:

Use a IoC container to create and inject the service (INavigationmanager) in ViewModels/ views

Get The Navigation Service:

* **GetDefault**
* **GetNamed** for a navigation page registered with a key 


Navigation Service methods

* **PushAsync**
* **PushModalAsync**
* **PopAsync**
* **PopModalAsync**
* **PopToRootAsync**

```cs
navigationManager.GetDefault().PushAsync(typeof(HomePage));

// with parameter
navigationManager.GetDefault().PushAsync(typeof(HomePage), "My parameter");


// with named for example a navigation page for a master detail page
navigationManager.GetNamed("MasterDetail").PushAsync(typeof(ItemDetailPage), "My parameter");

// pop
navigationManager.GetDefault().PopAsync(typeof(HomePage));

// push modal
navigationManager.GetDefault().PushModalAsync(typeof(PageA));

// pop modal
navigationManager.GetDefault().PopModalAsync();

// pop to root
navigationManager.GetDefault().PopToRootAsync();
```

Example for master detail page
```xml
<?xml version="1.0" encoding="utf-8" ?>
<MasterDetailPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="NavigationSample.Views.MyMasterDetailPage"
             xmlns:views="clr-namespace:NavigationSample.Views"
             Title="Master Detail Page"
             MasterBehavior="Popover">

  <MasterDetailPage.Master>
    <views:MenuPage />
  </MasterDetailPage.Master>

  <MasterDetailPage.Detail>
    <NavigationPage x:Name="MasterDetail">
      <x:Arguments>
        <views:ItemsPage />
      </x:Arguments>
    </NavigationPage>
  </MasterDetailPage.Detail>

</MasterDetailPage>
```

# System go back button

The Navigation service handle system back button pressed (check activation and notify viewmodel). Its possible to unhandle system go back.

```cs
navigationManager.GetDefault().UnhandleSystemPagePopped();
```

**INavigationParameterKnowledge** interface allows the navigation service to retrieve parameter after system go back.

```cs
public class PageAViewModel : INavigationParameterKnowledge
{
    public object Parameter { get; set; }
}
```

**IPageKnowledge** allows to get **page** from the view model.


# ViewModel with guards

```cs
public class PageAViewModel : IActivatable, IDeactivatable
{
    private IDialogService dialogService;

    public PageAViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    public async Task<bool> CanActivateAsync(object parameter)
    {
        var result = await dialogService.DisplayAlertAsync("Activate?", "Activate PageA?", "Ok", "Cancel");
        return result;
    }

    public async Task<bool> CanDeactivateAsync()
    {
        var result = await dialogService.DisplayAlertAsync("Deactivate?", "Deactivate PageA?", "Ok", "Cancel");
        return result;
    }
}
```

## Bootstrapper

Create a Bootstrapper with MvvmLib Ioc Container. 

We could use any IoC container (Unity, StructureMap, etc.)

```cs
public abstract class MvvmLibBootstrapper : BootstrapperBase
{
    protected IInjector container;

    public MvvmLibBootstrapper(IInjector container)
    {
        this.container = container;
    }

    protected override void RegisterRequiredTypes()
    {
        container.RegisterInstance<IInjector>(container);
        container.RegisterSingleton<IDialogService, DialogService>();
    }

    protected override void SetViewFactory()
    {
        ViewResolver.SetViewFactory((viewType) => container.GetNewInstance(viewType));
    }

    protected override void SetViewModelFactory()
    {
        ViewModelLocationProvider.SetViewModelFactory((viewModelType) => container.GetInstance(viewModelType));
    }
}
```

```cs
public class Bootstrapper : MvvmLibBootstrapper
{
    public Bootstrapper(IInjector container)
        :base(container)
    { }

    protected override void ConfigureNavigation(Page shell)
    {
        var navigationService = NavigationManager.Register((NavigationPage)shell);

        // if check on activation is required
        // navigationService.PushAsync(typeof(HomePage), "My Home Page message", true);
    }

    protected override Page CreateShell()
    {
        return new NavigationPage(new HomePage());

        // if check on activation is required
        // return new NavigationPage();
    }

    protected override void InitializeShell(Page shell)
    {
        App.Current.MainPage = shell;
    }

    protected override void RegisterTypes()
    {
        container.RegisterSingleton<INavigationManager, NavigationManager>();
    }
}
```

And use it (App / net standard project)

```cs
public partial class App : Application
{
    public new static App Current => (App)Application.Current;

    public IInjector Injector { get; } 

    public App()
    {
        InitializeComponent();

        Injector = new Injector();

        var bootstrapper = new Bootstrapper(Injector);
        bootstrapper.Run();
    }

    protected override void OnStart()
    {
        // Handle when your app starts
    }

    protected override void OnSleep()
    {
        // Handle when your app sleeps
    }

    protected override void OnResume()
    {
        // Handle when your app resumes
    }
}
```

# EventToCommand Behavior

> Allows to bind an event to a command.

Sample:

```xml
<StackLayout>
    <ListView x:Name="ItemsListView"
            ItemsSource="{Binding Items}"
            VerticalOptions="FillAndExpand"
            HasUnevenRows="true"
            IsPullToRefreshEnabled="true">

        <ListView.Behaviors>
            <nav:EventToCommandBehavior EventName="ItemSelected" Command="{Binding SelectCommand}" />
        </ListView.Behaviors>
        
        <ListView.ItemTemplate>
            <DataTemplate>
                <ViewCell>
                    <StackLayout Padding="10">
                        <Label Text="{Binding Name}" 
                            LineBreakMode="NoWrap" 
                            Style="{DynamicResource ListItemTextStyle}" 
                            FontSize="16" />
                    </StackLayout>
                </ViewCell>
            </DataTemplate>
        </ListView.ItemTemplate>
    </ListView>
</StackLayout>
```

ViewModel

```cs
public class ItemsPageViewModel
{
    public ICommand SelectCommand { get; }

    public ItemsPageViewModel(INavigationManager navigationManager)
    {
        SelectCommand = new RelayCommand<SelectedItemChangedEventArgs>((t) =>
        {
            navigationManager.GetNamed("MasterDetail").PushAsync(typeof(ItemDetailPage), parameter: t.SelectedItem);
        });
    }
}
```