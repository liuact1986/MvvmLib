## MvvmLib.Wpf (Navigation) [net 4.5]

* **Regions**: change and animate the content of **ContentRegion** (ContentControl) and **ItemsRegions** (ItemsControl, TabControl, ... and more with Adapters) 
* **ViewModelLocator**: allows to resolve ViewModel for regions and for window with **ResolveWindowViewModel**
* **RegionManager**: allows to regiter a region with attached properties
* **RegionNavigationService** allows to **navigate** _with regions_ 
* **INavigatable**: allows the views and view models to be notified on navigate
* **IActivatable**, **IDeactivatable**: allow to cancel navigation
* **IIsLoaded**: allows to be notified when the view or window is loaded
* **IViewLifetimeStrategy**: Allows to get always the same instance of a view (Singleton) for a region
* **ISelectable**: allows to select a view 
* **IsSelected**: allows to be notifed from view model on selection changed for ItemsRegion with Selector (ListBox, TabControl, etc.)
* **BootstrapperBase**: bootstrapper base class
* **BindableObject**: Allows to bind a value or object to Value dependency property and be notified on value changed.
* **AnimatableContentControl**, **TransitioningContentControl**, **TransitioningItemsControl**: allow to animate on content change / insertion, etc.

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

### RegionNavigationService

#### With Content Region

Inject the region manager (view and / or view model)

```cs
public class ViewAViewModel
{
    private IRegionNavigationService regionNavigationService;

    public ViewAViewModel(IRegionNavigationService regionNavigationService)
    {
        this.regionNavigationService = regionNavigationService;
    }
}
```

And use it

```cs
// GetContentRegion returns the last region registered for this region name
await regionNavigationService.GetContentRegion("MyContentRegion").NavigateAsync(typeof(ViewA));

// and the control name to target a control
await regionNavigationService.GetContentRegion("MyContentRegion", "MyContentRegion1").NavigateAsync(typeof(ViewA));
```

with parameter

```cs
await regionNavigationService.GetContentRegion("MyContentRegion").NavigateAsync(typeof(ViewA), "my parameter");
```

with exit and entrance animations

```cs
await regionNavigationService.GetContentRegion("MyContentRegion").NavigateAsync(typeof(ViewA), EntranceTransitionType.FadeIn, ExitTransitionType.FadeOut);
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

**Animation with content control**

 [See MvvmLib.Animation.Wpf](https://romagny13.github.io/MvvmLib/Animation.htm)

#### With Items Region

AddAsync

```cs
await regionNavigationService.GetItemsRegion("MyItemsRegion").AddAsync(typeof(ViewA));

// by control name
await regionNavigationService.GetItemsRegion("MyItemsRegion","MyItemsRegion1").AddAsync(typeof(ViewA));

// with a parameter
await regionNavigationService.GetItemsRegion("MyItemsRegion").AddAsync(typeof(ViewA),"my parameter");

// with transition
await regionNavigationService.GetItemsRegion("MyItemsRegion").AddAsync(typeof(ViewA), EntranceTransitionType.SlideInFromRight);
```

InsertAsync

```cs
// example : index 2
await regionNavigationService.GetItemsRegion("MyItemsRegion").InsertAsync(2, typeof(ViewD));
```

RemoveLastAsync (remove the last item)

```cs
await regionNavigationService.GetItemsRegion("MyItemsRegion").RemoveLastAsync(ExitTransitionType.SlideOutToBottom);
```

RemoveAtAsync

```cs
// example : index 2
await WpfNavigationService.Default.GetItemsRegion(RegionNames.ItemsControlRegionName).RemoveAtAsync(2);
```

**ItemsRegion Methods** :

* **AddAsync**
* **InsertAsync**
* **RemoveAtAsync**, **Remove**, **Clear**
* **FindIndex**, **FindControlIndex**, **FindContextIndex**

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

## ICanActivate and ICanDeactivate Navigation Guards

Allow to cancel navigation (View and/or View model)

```cs
public class ViewAViewModel : ICanActivate, ICanDeactivate
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

## IIsLoaded

> Allows to be notifed from ViewModel when view is loaded.

```cs
public class ShellViewModel : IIsLoaded
{
    IRegionNavigationService regionNavigationService;

    public ShellViewModel(IRegionNavigationService regionNavigationService)
    {
        this.regionNavigationService = regionNavigationService;
    }

    public async void OnLoaded(object parameter)
    {
        await regionNavigationService.GetContentRegion("ContentRegion").NavigateAsync(typeof(HomeView));
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

    private IRegionNavigationService regionNavigationService;

    private IFakePeopleService fakePeopleService;

    public PersonDetailsViewModel(IRegionNavigationService regionNavigationService, IFakePeopleService fakePeopleService)
    {
        this.regionNavigationService = regionNavigationService;
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

## IIsSelected

> Allow to be notifed from ViewModel on selection changed event (SelectedItems) for ItemsRegion with a Selector control (ListBox, TabControl, etc.)

```cs
public class ViewCViewModel : BindableBase, IIsSelected
{
    private string message;
    public string Message
    {
        get { return message; }
        set { SetProperty(ref message, value); }
    }

    private bool isSelected;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            SetProperty(ref isSelected, value);
            if (isSelected)
                Message = "ACTIVE";
            else
                Message = "NOT Active";
        }
    }
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


## Create a custom  items region Adapter

Implement IItemsRegionAdapter.

Example for a StackPanel

```cs
public class StackPanelRegionAdapter : IItemsRegionAdapter
{
    private StackPanel control;
    public DependencyObject Control
    {
        get { return control; }
        set
        {
            if (value is StackPanel stackPanel)
                control = stackPanel;
            else
                throw new InvalidOperationException("Invalid control type");
        }
    }

    public void Adapt(ItemsRegion region)
    {
        if (region == null)
            throw new ArgumentNullException(nameof(region));

        region.History.Entries.CollectionChanged += OnEntriesCollectionChanged;
    }

    private void OnEntriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
       // update the stackpanel children ...
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
        RegionAdapterContainer.RegisterAdapter(typeof(StackPanel), new StackPanelRegionAdapter()); // <===
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

## AnimatableContentControl

> Content Control that allows to animate on content change. 

2 Storyboards : 

* EntranceAnimation 
* ExitAnimation
* Simultaneous (boolean) allow to play simultaneously the animations.

EntranceAnimation: Target "CurrentContentPresenter" 
ExitAnimation: Target "CurrentContentPresenter" or with Simulatenous "PreviousContentPresenter"


```xml
<mvvmLib:AnimatableContentControl x:Name="AnimatableContentControl1" 
                                    mvvmLib:RegionManager.ContentRegionName="AnimationSample" 
                                    Simultaneous="True"
                                    IsCancelled="{Binding IsCancelled}"
                                    Grid.Row="1">
    <mvvmLib:AnimatableContentControl.ExitAnimation>
        <Storyboard>
            <!-- 1. translate -->
            <DoubleAnimation  Storyboard.TargetName="PreviousContentPresenter"
                                Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"
                                From="0" To="400" 
                                Duration="{Binding ElementName=DuractionComboBox,Path=SelectedItem}">
                <DoubleAnimation.EasingFunction>
                    <SineEase EasingMode="EaseInOut" />
                </DoubleAnimation.EasingFunction>
            </DoubleAnimation>
        </Storyboard>
    </mvvmLib:AnimatableContentControl.ExitAnimation>
    <mvvmLib:AnimatableContentControl.EntranceAnimation>
        <Storyboard>
            <!-- 1. translate -->
            <DoubleAnimation  Storyboard.TargetName="CurrentContentPresenter"
                                Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"
                                From="400" To="0" 
                                Duration="{Binding ElementName=DuractionComboBox,Path=SelectedItem}">
                <DoubleAnimation.EasingFunction>
                    <SineEase EasingMode="EaseInOut" />
                </DoubleAnimation.EasingFunction>
            </DoubleAnimation>
        </Storyboard>
    </mvvmLib:AnimatableContentControl.EntranceAnimation>
</mvvmLib:AnimatableContentControl>
```

## TransitioningContentControl

> Allows to play a transition on loaded.

2 Storyboards:

* EntranceTransition: played when control loaded (or explicitly with "DoEnter")
* ExitTransition: played explicitly with "DoLeave" or IsLeaving dependency property (for example played when the user click on a tab close button)

Other methods:

* CancelTransition
* Reset: reset the render transform property and opacity + cancel transition

```xml
<mvvmLib:TransitioningContentControl x:Name="TransitioningContentControl1" Margin="0,20">
        <mvvmLib:TransitioningContentControl.EntranceTransition>
            <Storyboard>
                <DoubleAnimation Storyboard.TargetName="ContentPresenter" 
                                    Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)" 
                                    From="0" To="1" Duration="0:0:0.6">
                    <DoubleAnimation.EasingFunction>
                        <ExponentialEase EasingMode="EaseInOut"/>
                    </DoubleAnimation.EasingFunction>
                </DoubleAnimation>
            </Storyboard>
        </mvvmLib:TransitioningContentControl.EntranceTransition>
        <mvvmLib:TransitioningContentControl.ExitTransition>
            <Storyboard>
                <DoubleAnimation Storyboard.TargetName="ContentPresenter" 
                                    Storyboard.TargetProperty="(UIElement.Opacity)" 
                                    From="1" To="0" Duration="0:0:2"/>
            </Storyboard>
</mvvmLib:TransitioningContentControl.ExitTransition>
```

## TransitioningItemsControl

> ItemsControl that allows to animate on item insertion and deletion. 

The "ControlledAnimation" avoid to set the target and the target property of the storyboard. The TargetPropertyType is a shortcut. But it's possible to target explicitly the target property of the storyboard with "TargetProperty" dependency property.

```xml
<mvvmLib:TransitioningItemsControl x:Name="I2"
                                        ItemsSource="{Binding MyItems}" 
                                        TransitionClearHandling="Transition"
                                        IsCancelled="{Binding IsCancelled}">
    <mvvmLib:TransitioningItemsControl.EntranceAnimation>
        <mvvmLib:ParallelAnimation>

            <mvvmLib:ControlledAnimation TargetPropertyType="TranslateX">
                <DoubleAnimation From="200" To="0"  Duration="0:0:2"/>
            </mvvmLib:ControlledAnimation>

        </mvvmLib:ParallelAnimation>
    </mvvmLib:TransitioningItemsControl.EntranceAnimation>

    <mvvmLib:TransitioningItemsControl.ExitAnimation>
        <mvvmLib:ParallelAnimation>
            <mvvmLib:ControlledAnimation TargetPropertyType="TranslateX">
                <DoubleAnimation From="0" To="200" Duration="0:0:2"/>
            </mvvmLib:ControlledAnimation>
        </mvvmLib:ParallelAnimation>
    </mvvmLib:TransitioningItemsControl.ExitAnimation>
</mvvmLib:TransitioningItemsControl>
```