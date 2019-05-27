## MvvmLib.Wpf (Navigation) [net 4.5]

* **NavigationSource**: navigation for _ContentControl_
* **SharedSource**: for _ItemsControl_, _Selector_, etc.
* **AnimatableContentControl**, **TransitioningContentControl**, **TransitioningItemsControl**: allow to animate content
* **NavigationManager**: allows to manage NavigationSources and SharedSources
* **INavigatable**: allows views and _view models_ to be notified on navigate
* **ICanActivate**, **ICanDeactivate**: allow to cancel navigation
* **IIsSelected**, **ISelectable**, **SelectionChangedBehavior**: allow to select a view 
* **Navigation Behaviors**: **SelectionChangedBehavior** and **EventToCommandBehavior**
* **ViewModelLocator**: allows to **resolve ViewModel** for **views**
* **BootstrapperBase**: base class for Bootstrapper

## ViewModelLocator

> Allows to resolve ViewModels for Views with **ResolveViewModel**. 

Default **convention**:

* Views in `Views` namespace
* View models in `ViewModels` namespace
* View model name: 
    * _view name + "ViewModel"_ (example: ShellViewModel for Shell)
    * Or if the view name ends by "View": _view name + "Model"_ (example: NavigationViewModel for NavigationView)


### Change the convention

Example with "View" and "ViewModel" namespaces

```cs
ViewModelLocationProvider.ChangeConvention((viewType) =>
{
    var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

    var viewName = viewType.FullName;
    viewName = viewName.Replace(".View.", ".ViewModel.");
    var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
    var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);

    var viewModelType = Type.GetType(viewModelName);
    return viewModelType;
});
```

### Register a custom View Model for a view

For example: the view model for a ViewA is not ViewAViewModel but MyCustomViewAViewModel

```cs
ViewModelLocationProvider.RegisterCustom(typeof(ViewA), typeof(MyCustomViewAViewModel));
```

### ResolveViewModel Attached property (Window, UserControl)

Allows to resolve the view model of the Views. Example:


```xml
<Window x:Class="Sample.Views.Shell"
        ...
         xmlns:nav="http://mvvmlib.com/"
         nav:ViewModelLocator.ResolveViewModel="True">
```

**Note**: NavigationSources and Shared Sources resolve automatically the ViewModel with the ViewModelLocator. So, using "ResolveViewModel" attached property is rarely required.


## NavigationSource and ContentControlNavigationSource

### NavigationSource

The NavigationSource is not linked to the UI. So its possible to create all the navigation sources required by the application at Startup.


Example: Creating some Navigation sources in ShellViewModel

```cs
public class ShellViewModel
{
    public NavigationSource Navigation { get; }

    public ShellViewModel()
    {     
        Navigation = NavigationManager.CreateNavigationSource("Main");

        NavigationManager.CreateNavigationSource("Details");

        NavigationManager.CreateNavigationSource("AnimationSample");

        NavigationManager.CreateNavigationSource("HistorySample");
    }
}
```

Get a NavigationSource:

```cs
var navigation = NavigationManager.GetNavigationSource("Main");
```

Or 

```cs
var navigation = NavigationManager.GetOrCreateNavigationSource("Main");
```

Bind the NavigationSource **Current** property to a **ContentControl**

```xml
<ContentControl Content="{Binding Navigation.Current}" />
```

The NavigationSource provide some quick Commands

```xml
<!--Navigate command -->
<Button Content="View A" Command="{Binding Navigation.NavigateCommand}" CommandParameter="{x:Type views:ViewA}" />

<!--GoBack command -->
<Button Content="Go Back" Command="{Binding Navigation.GoBackCommand}" />

<!--GoForward command -->
<Button Content="Go Forward" Command="{Binding Navigation.GoForwardCommand}" />

<!--NavigateToRoot command -->
<Button Content="Root" Command="{Binding Navigation.NavigateToRootCommand}" />
```

### ContentControlNavigationSource

Inherits from NavigationSource and updates directly the content of the ContentControl.


```xml
<ContentControl mvvmLib:NavigationManager.SourceName="Main" />
```

The namespace:

```xml
<UserControl ...
            xmlns:mvvmLib="http://mvvmlib.com/">
```

## INavigatable

> Allows to notify ViewModel with parameter

* **OnNavigatingFrom**
* **OnNavigatingTo**
* **OnNavigatedTo**

```cs
public class ViewAViewModel : INavigatable
{
    public void OnNavigatingTo(object parameter)
    {

    }

    public void OnNavigatedTo(object parameter)
    {

    }

    public void OnNavigatingFrom()
    {

    }
}
```

| Method | Description |
| --- | --- |
| NavigateAsync | Allows to navigate to a view or view model (with DataTemplate) (parameters: source type, parameter) |
| GoBackAsync | Allows to go to the previous view |
| GoForwardAsync | Allows to go the next view |
| NavigateToRootAsync | Allows to navigate to the first view/ root view |
| RedirectAsync | allows to redirect to a view and do not add/remove current page from history |


```cs
var  navigation = NavigationManager.CreateNavigationSource("Main");

await navigation.NavigateAsync(typeof(ViewA));

// with parameter
await navigation.NavigateAsync(typeof(ViewA), "My parameter");

// GoBack
await navigation.GoBackAsync();

// GoForward
await navigation.GoForwardAsync();

// Navigate to root
await navigation.NavigateToRootAsync();
```

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


## Navigation Guards (ICanActivate, ICanDeactivate)

Useful when the user want to leave a page or close a tabitem for example.

```cs
public class ViewAViewModel : ICanActivate, ICanDeactivate
{

    public Task<bool> CanActivateAsync(object parameter)
    {
        var result = MessageBox.Show("Can activate?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        return Task.FromResult(result);
    }

    public Task<bool> CanDeactivateAsync()
    {
        var result = MessageBox.Show("Can deactivate?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        return Task.FromResult(result);
    }
}
```

## ISelectable with NavigationSource

Allows to use the "same view" multiple times. For example in a Master/ details scenario, the details view is used for displaying each user.

```cs
public class PersonDetailsViewModel : BindableBase, ISelectable 
{
    private Person person;
    public Person Person
    {
        get { return person; }
        set { SetProperty(ref person, value); }
    }

    // etc.

    public bool IsTarget(Type viewType, object parameter)
    {
        if (parameter != null)
            return person.Id == (int)parameter;
        
        return false;
    }
}
```

## SharedSource for ItemsControl, ListBox, TabControl, etc.

Provides an Items collection that implements INotifyCollectionChanged, a SelectedItem and more to quickly bind sources... add, remove, replace, move items, select an item by SelectedIndex or SelectedItem, etc. INotifyCollectionChanged and INotifyPropertyChanged do the work for the UI.

```cs
public class ViewAViewModel
{
    public SharedSource<ItemDetailsViewModel> DetailsSource { get; }

    public ICommand AddCommand { get; }

    public  ViewAViewModel()
    {

        // empty
        // DetailsSource = NavigationManager.GetOrCreateSharedSource<ItemDetailsViewModel>();

        // or with data at initialization
        DetailsSource = NavigationManager.GetOrCreateSharedSource<ItemDetailsViewModel>().With(new List<ItemDetailsViewModel>
        {
            new ItemDetailsViewModel(new Item { Name = "Item.1" }),
            new ItemDetailsViewModel(new Item { Name = "Item.2" })
        });

        AddCommand = new RelayCommand(Add);
    }

    private async void Add()
    {
        await DetailsSource.Items.AddAsync(new ItemDetailsViewModel(new Item { Name = $"Item.{DetailsSource.Items.Count + 1}" }));
    }
}
```

Bind the source to controls:

ItemsControl

```xml
<ItemsControl ItemsSource="{Binding DetailsSource.Items}" />
```

Selector (ListBox, TabControl, etc.) :

```xml
<ListView ItemsSource="{Binding DetailsSource.Items}" SelectedItem="{Binding DetailsSource.SelectedItem}" />
```

```xml
<TabControl ItemsSource="{Binding DetailsSource.Items}"  SelectedItem="{Binding DetailsSource.SelectedItem}">
    <TabControl.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="{Binding Item.Name}" Margin="20,0"/>
                <Button Content="X" 
                        Command="{Binding CloseCommand}" 
                        HorizontalAlignment="Right" Height="20" Width="20" 
                        VerticalAlignment="Top" />
            </StackPanel>
        </DataTemplate>
    </TabControl.ItemTemplate>
</TabControl>
```

**Tip**: Use an interface (IDetailViewModel for example) for TabControl that can display multiple views.

```cs
public interface IDetailsViewModel
{
    string Title { get; set; }
}

public abstract class DetailsViewModelBase : BindableBase, IDetailsViewModel
{
    protected string title;
    public string Title
    {
        get { return title; }
        set { SetProperty(ref title, value); }
    }

    public SharedSource<IDetailsViewModel> DetailsSource { get; }

    public DetailsViewModelBase()
    {
        DetailsSource = SharedSourceManager.GetOrCreate<IDetailsViewModel>();
    }
}

public class ViewAViewModel : DetailsViewModelBase, ICanDeactivate
{
    // ...
}
public class ViewBViewModel : DetailsViewModelBase
{
    // ...
}
```

## IIsSelected, ISelectable and SelectionChangedBehavior

Bind the Items collection and the SelectedItem of the SharedSource is easy.

**IIsSelected** allows to be notified from ViewModel of selection.


```cs
public class ViewCViewModel : DetailsViewModelBase, IIsSelected
{
    private bool isSelected;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            SetProperty(ref isSelected, value);
            if (isSelected)
                Title = "ACTIVE";
            else
                Title = "NOT Active";
        }
    }
}
```

**ISelectable** allows for example to select a tabitem opened

```cs
public class ViewDViewModel : DetailsViewModelBase, ISelectable
{
    public bool IsTarget(Type sourceType, object parameter)
    {
        return sourceType == typeof(ViewDViewModel);
    }
}
```

**SelectionChangedBehavior** allows to notify all ViewModels (that implements IIsSelected) for a ListView with selection mode Multiple for example.

```xml
<ListView x:Name="ListView1" 
            ItemsSource="{Binding DetailsSource.Items}"
            SelectedItem="{Binding DetailsSource.SelectedItem}"
            SelectionMode="Multiple" 
            ItemContainerStyle="{StaticResource ListViewItemStyle}">
    <mvvmLib:NavigationInteraction.Behaviors>
        <mvvmLib:SelectionChangedBehavior />
    </mvvmLib:NavigationInteraction.Behaviors>
</ListView>
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


## AnimatableContentControl

> Content Control that allows to animate on content change. 

2 Storyboards : 

* EntranceAnimation 
* ExitAnimation
* Simultaneous (boolean) allows to play simultaneously the animations.

EntranceAnimation: Target "CurrentContentPresenter" 
ExitAnimation: Target "CurrentContentPresenter" or with Simulatenous "PreviousContentPresenter"


```xml
<mvvmLib:AnimatableContentControl mvvmLib:NavigationManager.SourceName="Main">
    <mvvmLib:AnimatableContentControl.EntranceAnimation>
        <Storyboard>
            <DoubleAnimation Storyboard.TargetName="CurrentContentPresenter" 
                             Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"
                             From="400" To="0" Duration="0:0:0.4"  />
        </Storyboard>
    </mvvmLib:AnimatableContentControl.EntranceAnimation>
    <mvvmLib:AnimatableContentControl.ExitAnimation>
        <Storyboard>
            <DoubleAnimation Storyboard.TargetName="CurrentContentPresenter" 
                             Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"
                             From="0" To="400" Duration="0:0:0.4"  />
        </Storyboard>
    </mvvmLib:AnimatableContentControl.ExitAnimation>
</mvvmLib:AnimatableContentControl>
```

Or Simulatneous

```xml
  <mvvmLib:AnimatableContentControl Content="{Binding Navigation.Current}" 
                                    Simultaneous="True"
                                    IsCancelled="{Binding IsCancelled}">
    <mvvmLib:AnimatableContentControl.ExitAnimation>
        <Storyboard>
            <DoubleAnimation  Storyboard.TargetName="PreviousContentPresenter"
                                Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"
                                From="0" To="{Binding ElementName=ThisControl,Path=ActualWidth,FallbackValue=400}" 
                                Duration="{Binding ElementName=DuractionComboBox,Path=SelectedItem}">
                <DoubleAnimation.EasingFunction>
                    <SineEase EasingMode="EaseInOut" />
                </DoubleAnimation.EasingFunction>
            </DoubleAnimation>
        </Storyboard>
    </mvvmLib:AnimatableContentControl.ExitAnimation>
    <mvvmLib:AnimatableContentControl.EntranceAnimation>
        <Storyboard>
            <DoubleAnimation  Storyboard.TargetName="CurrentContentPresenter"
                                Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"
                                From="{Binding ElementName=ThisControl,Path=ActualWidth,FallbackValue=400}" To="0" 
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
<mvvmLib:TransitioningItemsControl ItemsSource="{Binding MyItems}" 
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

## EventToCommandBehavior

> Allows to bind an event to a command.

Sample button "Click" event binded to a ViewModel command

```xml
<Button Content="Click event">
    <mvvmLib:NavigationInteraction.Behaviors>
        <mvvmLib:EventToCommandBehavior EventName="Click" Command="{Binding SayHelloCommand}" CommandParameter="World"/>
    </mvvmLib:NavigationInteraction.Behaviors>
</Button>
```

ViewModel

```cs
public class ViewAViewModel : BindableBase
{
    private string message;
    public string Message
    {
        get { return message; }
        set { SetProperty(ref message, value); }
    }

    public ICommand SayHelloCommand { get; }

    public ViewAViewModel()
    {
        SayHelloCommand = new RelayCommand<string>(SayHello);
    }

    private void SayHello(string value)
    {
        Message = $"Hello {value}! {DateTime.Now.ToLongTimeString()}";
    }
}
```