## MvvmLib.Wpf (Navigation) [net 4.5]

* **Regions**: change and animate the content of **ContentRegion** (ContentControl) and **ItemsRegions** (ItemsControl, TabControl, ... and more with Adapters) 
* **ViewModelLocator**: allows to resolve ViewModel for regions and for window with **ResolveWindowViewModel**
* **RegionManager**: allows to get a region, then **navigate** _with regions_ 
* **INavigatable**: allows the views and view models to be notified on navigate
* **IActivatable**, **IDeactivatable**: allow to cancel navigation
* **ILoadedEventListener**: allows to be notified when the view or window is loaded
* **IViewLifetimeStrategy**: Allows to get always the same instance of a view (Singleton) for a region
* **ISelectable**: allows to select a view 
* **BootstrapperBase**: bootstrapper base class
* **BindableObject**: Allows to bind a value or object to Value dependency property and be notified on value changed.

## ViewModelLocator

> Allows to resolve ViewModels for regions and window (with **ResolveWindowViewModel**)

Default **convention**:

* Views in `Views` namespace
* View models in `ViewModels` namespace
* View model name: 
    * _view name + "ViewModel"_ (example: ShellViewModel for Shell)
    * Or if the view name ends by "View": _view name + "Model"_ (example: NavigationViewModel for NavigationView)


### Change the convention

Example with "View" and "ViewModel" namespaces

```cs
 ViewModelLocationProvider.SetViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

                var viewName = viewType.FullName;
                viewName = viewName.Replace(".View.", ".ViewModel."); // <===
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);

                return Type.GetType(viewModelName);
            });
```

### Register a custom View Model for a view

For example: the view model for a ViewA is not ViewAViewModel but MyCustomViewAViewModel

```cs
ViewModelLocationProvider.RegisterCustom(typeof(ViewA), typeof(MyCustomViewAViewModel));
```

### ResolveWindowViewModel Attached property (Window)

Allows to resolve the view model of the Window. Example:

**Only on Windows** (Shell or "popup")

```xml
<Window x:Class="WpfLibSample.Views.Shell"
        ...
         xmlns:nav="http://mvvmlib.com/"
         nav:ViewModelLocator.ResolveWindowViewModel="True">
```

## Regions 

namespace
```
xmlns:nav="http://mvvmlib.com/"
```

With a ContentControl

```xml
<ContentControl x:Name="MyContentRegion1" nav:RegionManager.ContentRegionName="MyContentRegion"></ContentControl>
```

With an ItemsControl

```xml
<ItemsControl nav:RegionManager.ItemsRegionName="MyItemsRegion"></ItemsControl>
```

With a TabControl (with the Header binded to the property Title of the ViewModel for example)
```xml
<TabControl nav:RegionManager.ItemsRegionName="MyTabRegion">
            <TabControl.ItemContainerStyle>
                <Style TargetType="TabItem">
                    <Setter Property="Header" Value="{Binding Title}" />
                </Style>
            </TabControl.ItemContainerStyle>
</TabControl>
```

The control name could be used to resolve the region if more than one region with the same region name are registered 

Example:

```xml
<ContentControl x:Name="MyContentRegion1" nav:RegionManager.ContentRegionName="MyContentRegion"></ContentControl>
<ContentControl x:Name="MyContentRegion2" nav:RegionManager.ContentRegionName="MyContentRegion"></ContentControl>
```

**Tip**: create a class with region names

```cs
internal class RegionNames
{
    public static string ContentControlRegionName = "ContentControlRegion";
    public static string ItemsControlRegionName = "ItemsControlRegion";
    public static string TabControlRegionName = "TabControlRegion";
    public static string StackPanelRegionName = "StackPanelRegionName";
}
```
... And change the name

```xml
<ContentControl nav:RegionManager.ContentRegionName="{x:Static local:RegionNames.ContentControlRegionName}"></ContentControl>
```

### Navigate

#### With Content Region

Inject the region manager (view and / or view model)

```cs
public class ViewAViewModel
{
    IRegionManager regionManager;

    public ViewAViewModel(IRegionManager regionManager)
    {
        this.regionManager = regionManager;
    }
}
```

And use it

```cs
// GetContentRegion returns the last region registered for this region name
await regionManager.GetContentRegion("MyContentRegion").NavigateAsync(typeof(ViewA));

// and the control name to target a control
await regionManager.GetContentRegion("MyContentRegion", "MyContentRegion1").NavigateAsync(typeof(ViewA));
```

with parameter

```cs
await regionManager.GetContentRegion("MyContentRegion").NavigateAsync(typeof(ViewA), "my parameter");
```

with exit and entrance animations

```cs
await regionManager.GetContentRegion("MyContentRegion").NavigateAsync(typeof(ViewA), EntranceTransitionType.FadeIn, ExitTransitionType.FadeOut);
```

**Content Region**

| Method | Description |
| --- | --- |
| NavigateAsync | Allows to navigate to a view or view model (with DataTemplate) (parameters: page type, parameter and navigation transition type) |
| GoBackAsync | Allows to go to the previous view |
| GoForwardAsync | Allows to go the next view |
| NavigateToRootAsync | Allows to navigate to the first view/ root view |
| RedirectAsync | allows to redirect to a view and do not add/remove current page from history |


| Property | Description |
| --- | --- |
| History | Navigation History with notification on add and remove entry |
| CanGoBack | Returns true if back stack history have more then one entry |
| CanGoForward | Returns true if forward stack history have one or more entries |


| Event | Description |
| --- | --- |
| CanGoBackChanged | Invoked after can go back changed |
| CanGoForwardChanged | Invoked after can go forward changed |
| Navigating | Invoked before navigation starts |
| Navigated | Invoked after navigation ends |
| NavigatingFailed | Invoked after navigation was cancelled |

#### With Items Region

AddAsync

```cs
await regionManager.GetItemsRegion("MyItemsRegion").AddAsync(typeof(ViewA));

// by control name
await regionManager.GetItemsRegion("MyItemsRegion","MyItemsRegion1").AddAsync(typeof(ViewA));

// with a parameter
 await regionManager.GetItemsRegion("MyItemsRegion").AddAsync(typeof(ViewA),"my parameter");

// with transition
 await regionManager.GetItemsRegion("MyItemsRegion").AddAsync(typeof(ViewA), EntranceTransitionType.SlideInFromRight);
```

InsertAsync

```cs
// example : index 2
await regionManager.GetItemsRegion("MyItemsRegion").InsertAsync(2, typeof(ViewD));
```

RemoveLastAsync (remove the last item)

```cs
await regionManager.GetItemsRegion("MyItemsRegion").RemoveLastAsync(ExitTransitionType.SlideOutToBottom);
```

RemoveAtAsync

```cs
// example : index 2
await WpfNavigationService.Default.GetItemsRegion(RegionNames.ItemsControlRegionName).RemoveAtAsync(2);
```

**ItemsRegion Methods** :

* **AddAsync**
* **InsertAsync**
* **RemoveAtAsync**
* **RemoveLastAsync**
* **Clear**

### Animation

Define an entrance and an exit animation. Example:

```cs
var entranceScaleAnimation = new ScaleAnimation
{
    From = 0,
    To = 1,
    RenderTransformOrigin = new Point(0.5, 0.5),
    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }
};
var exitScaleAnimation = new ScaleAnimation
{
    From = 1,
    To = 0,
    RenderTransformOrigin = new Point(0.5, 0.5),
    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }
};

var region = regionManager.GetContentRegion("ContentRegion");
 region.ConfigureAnimation(entranceScaleAnimation, exitScaleAnimation);
```

Playing the entrance and exit animations simultaneously:

```cs
 region.ConfigureAnimation(entranceScaleAnimation, exitScaleAnimation, true);
 ```

 Animation classes awailables:

* **OpacityAnimation**
* **TranslateAnimation**
* **SkewAnimation**
* **ScaleAnimation**
* **RotateAnimation**
* **FxCornerEntranceAnimation** and **FxCornerExitAnimation**
* **FallEntranceAnimation** and **FallExitAnimation**
* **FxVscaleEntranceAnimation** and **FxVscaleExitAnimation**

Create a custom animation class:

```cs
// 1. inherit from ContentAnimationBase or TransformAnimationBase
public class MyAnimation : TransformAnimationBase
{
    // 2. implement the base class
    public override void CancelAnimation()
    {
        throw new System.NotImplementedException();
    }

    protected override void BeginAnimation()
    {
        throw new System.NotImplementedException();
    }

    protected override AnimationTimeline CreateAnimation()
    {
        throw new System.NotImplementedException();
    }
}
```

Example with the OpacityAnimation class:

```cs
public class OpacityAnimation : ContentAnimationBase
{
    protected override double DefaultFrom => 0;
    protected override double DefaultTo => 1;

    public override void CancelAnimation()
    {
        // on animation cancelled
        if (Element != null)
        {
            Element.BeginAnimation(Control.OpacityProperty, null);
            AnimationWasCancelled = true;
            IsAnimating = false;
        }
    }

    protected override AnimationTimeline CreateAnimation()
    {
        // return an animation
        var animation = new DoubleAnimation(From, To, Duration);
        if (EasingFunction != null)
            animation.EasingFunction = EasingFunction;
        return animation;
    }

    protected override void BeginAnimation()
    {
        // begin the animation
        Element.BeginAnimation(Control.OpacityProperty, Animation);
    }
}
```

## INavigatable

For View and/or View model

Example:

```cs
public class ViewAViewModel : INavigatable
{
    // Allows to preload data before region content has changed
    public void OnNavigatingTo(object parameter)
    {
        
    }

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

## IActivatable and IDeactivatable Navigation Guards

Allow to cancel navigation (View and/or View model)

```cs
public class ViewAViewModel : IActivatable, IDeactivatable
{
    public Task<bool> CanActivateAsync(object parameter)
    {
        var result = MessageBox.Show("Activate View A?", "Activate (VIEWMODEL)", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        return Task.FromResult(result);
    }

    public Task<bool> CanDeactivateAsync()
    {
        var result = MessageBox.Show("Deactivate View A?", "Deactivate", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        return Task.FromResult(result);
    }
}
```

## ILoadedEventListener

> Allows to be notifed from ViewModel when view is loaded.

```cs
public class ShellViewModel : ILoadedEventListener
{
    IRegionManager regionManager;

    public ShellViewModel(IRegionManager regionManager)
    {
        this.regionManager = regionManager;
    }

    public async void OnLoaded(object parameter)
    {
        await regionManager.GetContentRegion("ContentRegion").NavigateAsync(typeof(HomeView));
    }
}
```

## ISelectable

Allows to select an existing item and not re-create a new item (for example with a tabcontrol). 


Example with ItemsRegion. Select a TabITem for a TabControl 

```cs
public class MyTabViewModel : ISelectable
{
    public string Title => "My Tab";

    public bool IsTarget(Type viewType, object parameter)
    {
       // check the view type or the navigation parameter (id) for example
        return viewType == typeof(MyTabView);
    }
}
```

Example with Content Region

```cs
public class PersonDetailsViewModel : BindableBase, INavigatable, ISelectable 
{
    private Person person;
    public Person Person
    {
        get { return person; }
        set { SetProperty(ref person, value); }
    }

    private IRegionManager regionManager;

    private IFakePeopleService fakePeopleService;

    public PersonDetailsViewModel(IRegionManager regionManager, IFakePeopleService fakePeopleService)
    {
        this.regionManager = regionManager;
        this.fakePeopleService = fakePeopleService;
    }

    public void OnNavigatingFrom()
    {

    }

    public void OnNavigatingTo(object parameter)
    {
        int id = (int)parameter;
        var person = fakePeopleService.GetPersonById(id);
        Person = person;
    }

    public void OnNavigatedTo(object parameter)
    {

    }

    // we have a list of active views
    // select the view (note do not use Singleton with IViewLifetimeStrategy for this scenario)
    public bool IsTarget(Type viewType, object parameter)
    {
        if (parameter != null)
        {
            return person.Id == (int)parameter; // pass the id as parameter
        }
        return false;
    }
}
```

## IViewLifetimeStrategy

> Allows to get always the same instance of a view (Singleton) for a region.


```cs
public class PersonDetailsViewModel : IViewLifetimeStrategy
{
    public StrategyType Strategy => StrategyType.Singleton;

    // etc.
}
```

## Create a Bootsrapper


Example: using MvvmLib.IoC Container (or Unity, StructureMap, etc.)

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
        container.RegisterSingleton<IEventAggregator, EventAggregator>();
        container.RegisterSingleton<IRegionManager, RegionManager>();
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

The implementation class

```cs
public class Bootstrapper : MvvmLibBootstrapper
{
    public Bootstrapper(IInjector container) 
        : base(container)
    {  }

    protected override Window CreateShell()
    {
        return container.GetInstance<Shell>();
    }

    protected override void RegisterTypes()
    {
        container.RegisterSingleton<IFakePeopleService, FakePeopleService>();
    }
}
```

(App)

**Replace StartupUri** by the **Startup event**

```xml
<Application ...
             Startup="Application_Startup">

</Application>
```

```cs
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var bootstrapper = new Bootstrapper(new Injector());
        bootstrapper.Run();
    }
}
```


## Create a region Adapter

* ItemsRegionAdapter for controls with a collection of items or children (ItemsControl, ListView, TabControl,... StackPanel)

Example for a StackPanel

```cs
using System.Windows;
using System.Windows.Controls;
using WpfLib.Navigation;

namespace RegionSample.Adapters
{
    public class StackPanelRegionAdapter : ItemsRegionAdapterBase<StackPanel>
    {
        public override void OnClear(StackPanel control)
        {
            control.Children.Clear();
        }

        public override void OnInsert(StackPanel control, object view, int index)
        {
            if (index >= 0 && index <= control.Children.Count)
            {
                control.Children.Insert(index, (UIElement)view);
            }
        }

        public override void OnRemoveAt(StackPanel control, int index)
        {
            if (index >= 0 && index < control.Children.Count)
            {
                control.Children.RemoveAt(index);
            }
        }
    }
}
```

Then register this adapter (on Application Startup)

```cs
public class Bootstrapper : WpfLibBootstrapper
{
    protected override Window CreateShell()
    {
        return container.GetInstance<Shell>();
    }

    protected override void RegisterTypes()
    {
            container.RegisterSingleton<ViewBViewModel>();
    }

    protected override void RegisterCustomRegionAdapters()
    {
        RegionAdapterContainer.RegisterAdapter(new StackPanelRegionAdapter()); // <===
    }
}
```

And use it

```xml
<StackPanel nav:RegionManager.ItemsRegionName="{x:Static local:RegionNames.StackPanelRegionName}"></StackPanel>
```

## BindableObject<T>

> Allows to bind a value or object to `Value` dependency property and be notified on value changed.

Example 1:

```cs
var bindableObject = new BindableObject<string>();
bindableObject.PropertyChanged += (s, e) =>
{
    MessageBox.Show(e.PropertyName);
};
bindableObject.Value = "my value";
```

Example 2:

binding in Xaml

```xml
<TextBox Text="{Binding Value, UpdateSourceTrigger=PropertyChanged}"></TextBox>
```

```cs
var bindableObject = new BindableObject<string>();
bindableObject.PropertyChanged += (s, e) =>
{
    MessageBox.Show(e.PropertyName);
};
this.DataContext = bindableObject;
```
