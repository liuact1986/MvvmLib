## MvvmLib.Wpf (Navigation) [net 4.5]

* **BootstrapperBase**: allows to manage the startup of the application. Configure an IoC Container, the view and ViewModel factories, register dependencies, preload data, create the Shell ViewModel and the Shell.
* **NavigationSource**: Source for **Views** and **ViewModels**. It has an History, a collection of "Sources" (a source is a View or a ViewModel), a **Current** source that can be binded to the content of the **ContentControls**. There is two other NavigationSource Types: **KeyedNavigationSource** (a navigation source with a key) and **ContentControlNavigationSource** that updates directly the content of the ContentControl provided. The **SourceName Attached Property** of the NavigationManager allows to attach in Xaml a ContentControl to a ContentControlNavigationSource. The NavigationManager has a collection of NavigationSources stored with **NavigationSourceContainers**. It allows to **navigate simultaneously** for all NavigationSources registered for a SourceName and/or open new Shells.
* **SharedSource**: Source for **Models** and **ViewModels** with a collection of Items and SelectedItem/SelectedIndex. It supports Views but its not the target. This is the source for ItemsControls, Selectors (ListBox, TabControl), etc.
* **ListCollectionViewEx**: allows to browse, filter, sort, group, add, edit with lists and collections.
* **PagedSource**: paging for DataGrid, etc.
* **ViewModelLocator**: is used by **NavigationSources** and **SharedSources** (With CreateNew) to resolve the ViewModels for the Views. The default factory can be overridden and use an IoC Container to resolve dependencies. The **ResolveViewModel** attached property can be used on UserControls and Windows not used by the navigation to resolve the ViewModel and inject dependencies.
* **SourceResolver** is the factory for the views (FrameworkElements). It has to always create a new instance (and not use singletons) to avoid binding troubles.
* **NavigationManager**: allows to manage NavigationSources and SharedSources
* **INavigationAware**: allows _view models_ to be notified on navigate
* **ICanActivate**, **ICanDeactivate**: allow to cancel navigation
* **IIsSelected**, **ISelectable**, **SelectionSyncBehavior**: allow to select a view 
* **IIsLoaded**: allows to notify view model that the view is loaded for a view that use resolve view model attached property.
* **AnimatableContentControl**, **TransitioningContentControl**, **TransitioningItemsControl**: allow to animate content
* **Behaviors**: **SelectionSyncBehavior**, **EventToCommandBehavior**,**EventToMethodBehavior**
* **ModuleManager**: allows to manage modules/assemblies loaded "on demand"

## Create a Bootstrapper

Create a new Wpf application. Remove the MainWindow. Create a "Views" directory and a Window named "Shell".

Install the packages:

* **MvvmLib.Wpf** (**MvvmLib.Core** dependency is automatically installed)
* **MvvmLib.IoC** or another IoC container (Unity, Autofac, etc.)

Create a Bootstrapper Base Class

With **MvvmLib.IoC**

```cs
using MvvmLib.IoC;
using MvvmLib.Message;
using MvvmLib.Navigation;

namespace NavigationSample.Wpf.Startup
{
    public abstract class MvvmLibBootstrapper : BootstrapperBase
    {
        protected IInjector container;

        public MvvmLibBootstrapper(IInjector container)
        {
            if (container == null)
                throw new System.ArgumentNullException(nameof(container));

            this.container = container;
        }

        protected override void RegisterRequiredTypes()
        {
            container.RegisterInstance<IInjector>(container);
            container.RegisterSingleton<IEventAggregator, EventAggregator>();
        }

        protected override void SetViewFactory()
        {
            SourceResolver.SetFactory((sourceType) => container.GetNewInstance(sourceType));
        }

        protected override void SetViewModelFactory()
        {
            ViewModelLocationProvider.SetViewModelFactory((viewModelType) => container.GetInstance(viewModelType));
        }
    }
}
```

Create the Bootstrapper

```cs
public class Bootstrapper : MvvmLibBootstrapper
{
    public Bootstrapper(IInjector container)
        : base(container)
    { }

    protected override void RegisterTypes()
    {
        container.RegisterSingleton<IFakePeopleService, FakePeopleService>();
    }

    protected override void PreloadApplicationData()
    {
        NavigationManager.CreateDefaultNavigationSource("Main");
        NavigationManager.CreateDefaultNavigationSource("MasterDetails");

        NavigationManager.CreateSharedSource<MenuItem>();
        NavigationManager.CreateSharedSource<IDetailViewModel>();
        NavigationManager.CreateSharedSource<Person>("MasterDetails");
    }

    // its possible to define the ShellViewModel
    //protected override object CreateShellViewModel()
    //{
    //    return container.GetInstance<ShellViewModel>();
    //}

    protected override Window CreateShell()
    {
        return container.GetInstance<Shell>();
    }
}
```

App.Xaml: Remove "StartupUri="MainWindow.xaml" and add a startup event (Startup="Application_Startup")

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

## NavigationSource, KeyedNavigationSource and ContentControlNavigationSource

The NavigationSource is not linked to the UI. So its possible to create all the navigation sources required by the application at Startup.

The method CreateNavigationSource creates a container (for navigation sources with source name provided) and a first Navigation Source (returned by the function). The first naigation source created is a KeyedNavigationSource with the default key.

| Method | Description |
| --- | --- |
| Navigate | Navigates to the source (a source is a view or ViewModel) type or source name (for a type registered with SourceResolver.RegisterTypeForNavigation) |
| NavigateFast | Useful for navigation cancellation and not recheck guards |
| Redirect | Redirects and removes the previous entry from the history |
| MoveToFirst | Navigates to the first source and clears the history |
| MoveToPrevious | Navigates to the previous source |
| MoveToNext | Navigates to the next source |
| MoveToLast | Move to the last or the source |
| MoveTo | Move to the index or the (existing) source |
| Sync | Synchronizes the history and sources with the navigation source provided |

| Property | Description |
| --- | --- |
| Sources | The collection of sources |
| Current | The current source. Can be binded to Content property of ContentControls |
| CurrentIndex | The index of the current source |
| Entries | The navigation history entries |
| CanMoveToPrevious | Checks if can go back |
| CanMoveToNext | Checks if can go forward |
| ClearSourcesOnNavigate | By default the sources ("forward stack") are removed on navigate and navigate to root |

| Events | Description |
| --- | --- |
| PropertyChanged | Invoked on property changed (Current, CurrentIndex, etc.) |
| CollectionChanged | Invoked on collection changed (Sources) |
| CurrentChanged | Invoked on Current changed |
| CanMoveToPreviousChanged | Invoked on CanMoveToPrevious changed |
| CanMoveToNextChanged | Invoked on CanMoveToNext changed |
| Navigating | Invoked before navigation starts |
| Navigated | Invoked after navigation ends |
| NavigationFailed | Invoked on navigation cancelled |

Commands

* NavigateCommand with source type
* MoveToFirstCommand
* MoveToPreviousCommand
* MoveToNextCommand
* MoveToIndexCommand and MoveToCommand

Create the default navigation source.

```cs
this.Navigation = NavigationManager.CreateDefaultNavigationSource("Main");
```

Get the default navigation source

```cs
this.Navigation = NavigationManager.GetDefaultNavigationSource("Main");
```

Or ...

```cs
this.Navigation = NavigationManager.GetOrCreateDefaultNavigationSource("Main");
```

```cs
this.Navigation = new NavigationSource();
```

Navigation

```cs
var  navigation = NavigationManager.CreateNavigationSource("Main");

navigation.Navigate(typeof(ViewA));

// with parameter
navigation.Navigate(typeof(ViewA), "My parameter");

// MoveToPrevious
navigation.MoveToPrevious();

// MoveToNext
navigation.MoveToNext();

// Navigate to root ("forward stack" cleared)
navigation.MoveToFirst();
```

Add navigation sources for the same source name:

```cs
NavigationManager.AddNavigationSource("Main", new KeyedNavigationSource("MyKeyA"));
NavigationManager.AddNavigationSource("Main", new KeyedNavigationSource("MyKeyB"));
```

Navigate simultaneously with all sources of a container

```cs
var navigationSources = NavigationManager.GetNavigationSources("Main");
navigationSources.Navigate(typeof(ViewA), "My parameter");
```


The container provides some quick commands (these commands not check can go back / forward)

* NavigateCommand with source type
* MoveToFirstCommand
* MoveToPreviousCommand
* MoveToNextCommand
* RedirectCommand
* MoveToIndexCommand and MoveToCommand

Bind the NavigationSource **Current** property to a **ContentControl**

```xml
<ContentControl Content="{Binding Navigation.Current}" />
```

The NavigationSource provides some quick Commands

```xml
<!--Navigate command -->
<Button Content="View A" Command="{Binding Navigation.NavigateCommand}" CommandParameter="{x:Type views:ViewA}" />

<!--MoveToPrevious command -->
<Button Content="Go Back" Command="{Binding Navigation.MoveToPreviousCommand}" />

<!--MoveToNext command -->
<Button Content="Go Forward" Command="{Binding Navigation.MoveToNextCommand}" />

<!--MoveToFirst command -->
<Button Content="Root" Command="{Binding Navigation.MoveToFirstCommand}" />
```

Sources management

| Methods | Description |
| --- | --- |
| InsertNewSource | Creates the source with SourceResolver, inject dependencies and inserts the source at the index. The parameter is stored |
| AddNewSource | Creates the source with SourceResolver, inject dependencies and adds the source. The parameter is stored |
| RemoveSourceAt | Removes the source at the index |
| RemoveSource | Removes the source |
| RemoveSources | Removes the sources from the start index to the end (remove range) |
| ClearSources | Clears the source collection |


Example: insert and move to a source

```cs
var source = Navigation.InsertNewSource(0, typeof(ViewA), "View A Inserted at index 0");
Navigation.MoveTo(source);
// or by index
Navigation.MoveTo(0);
```

Navigation processes

| Process | Methods | Description |
| --- | --- | --- |
| "Navigate" | Navigate, NavigateFast, Redirect | Find the **selectable** (ISelectable) or create a **new** instance, **INavigationAware** methods invoked (OnNavigatingTo and OnNavigatedTo only for new instance) |
| "Move" | MoveTo, MoveToPrevious, MoveToNext, MoveToFirst, MoveToLast | Only **INavigationAware** methods invoked |


### ContentControlNavigationSource

Inherits from NavigationSource and updates directly the content of the ContentControl.


```xml
<ContentControl mvvmLib:NavigationManager.SourceName="Main" />
```

Registering navigation sources for the same source name :

```xml
<ContentControl mvvmLib:NavigationManager.SourceName="Main" />
<ContentControl mvvmLib:NavigationManager.SourceName="Main" />
<ContentControl mvvmLib:NavigationManager.SourceName="Main" />
```

The namespace:

```xml
<UserControl ...
            xmlns:mvvmLib="http://mvvmlib.com/">
```

## SharedSource for ItemsControl, ListBox, TabControl, etc.

Provides an Items collection that implements INotifyCollectionChanged, a SelectedItem and more to quickly bind sources... add, remove, replace, move items, select an item by SelectedIndex or SelectedItem, etc. INotifyCollectionChanged and INotifyPropertyChanged do the work for the UI.

Methods

| Method | Description |
| --- | --- |
| Load | Allows to initialize the SharedSource with a collection and parameters |
| CreateNew | Returns an item instance created with the SourceResolver |
| InsertNew | Creates a new instance with the SourceResolver an inserts the item created at index. A parameter can be provided for navigation |
| AddNew | Creates a new instance with the SourceResolver an inserts the item created at index. A parameter can be provided for navigation |
| Insert | Allows to insert item at index. ICanDeactive, ICanActivate and INavigationAware are invoked for items that implement these interfaces |
| Add | Adds and item. ICanDeactive, ICanActivate and INavigationAware are invoked for items that implement these interfaces |
| Move | Moves the item from the old index to the new index. Navigation guards and INavigationAware are not invoked |
| Replace | Replaces the old item at thhe index by the new item |
| RemoveAt | Removes the item at the index. ICanDeactive is checked for the item and OnNavigatingFrom is invoked for item that implement INavigationAware |
| Remove | Removes the item. ICanDeactive is checked for the item and OnNavigatingFrom is invoked for item that implement INavigationAware |
| Clear | Removes all items. ICanDeactive and INavigationAware OnNavigatingFrom methods are invoked for each item before deletion |
| ClearFast | Removes all items. ICanDeactive and INavigationAware methods are not invoked |
| Sync | Synchronizes the SharedSource with the SharedSource provided |

Properties

| Property | Description |
| --- | --- |
| SelectedItem | The selected item. Allows to bind quickly for Selectors (ListBox, TabControl, etc.) |
| SelectedIndex | The index of selected item |
| SelectionHandling | Allows to select automatically items after insertion, etc. (SelectedItem) |

Events

| Event | Description |
| --- | --- |
| PropertyChanged | Invoked on property changed (count, indexer) |
| SelectedItemChanged | Invoked on selected item changed |
| CanMoveToPreviousChanged | Invoked on CanMoveToPrevious changed |
| CanMoveToNextChanged | Invoked on CanMoveToNext changed |

Commands

* MoveToFirstCommand
* MoveToPreviousCommand
* MoveToNextCommand
* MoveToIndexCommand and MoveToCommand

### Creating SharedSources

```cs
var s = NavigationManager.CreateSharedSource<MySharedItem>();
```
... or locally

```cs
var s = new SharedSource<MySharedItem>();
```

Get a SharedSource already created

```cs
var s = NavigationManager.GetSharedSource<MySharedItem>();
```

Or use GetOrCreateSharedSource method

```cs
var s = NavigationManager.GetOrCreateSharedSource<MySharedItem>();
```

Creating SharedSources with keys. Allows to use the same type for multiple SharedSource.

```cs
var s1 = NavigationManager.CreateSharedSource<MySharedItem>("key1");
var s2 = NavigationManager.CreateSharedSource<MySharedItem>("key2");
```

GetSharedSource, GetOrCreateSharedSource, etc. methods are available with keys.

Remove a SharedSource

```cs
NavigationManager.RemoveSharedSource<MySharedItem>();
NavigationManager.RemoveSharedSource<MySharedItem>("key1");
```

Always get a new SharedSource for the type

```cs
var s = NavigationManager.GetNewSharedSource<MySharedItem>();
```

### Add items to a Shared source

Load

```cs
DetailsSource.Load(new List<MyItemDetailsViewModel>
{
    new MyItemDetailsViewModel(new MyItem { Name = "Item.1" }),
    new MyItemDetailsViewModel(new MyItem { Name = "Item.2" })
});
```

Or with parameters

```cs
DetailsSource.Load(new InitItemCollection<MyItemDetailsViewModel>
{
    { new MyItemDetailsViewModel(new MyItem { Name = "Item.1" }), "Parameter 1" },
    { new MyItemDetailsViewModel(new MyItem { Name = "Item.2" }), "Parameter 2" }
});
```

Adding items

```cs
var s = NavigationManager.GetSharedSource<MyViewModel>();
s.Add(1, new MyViewModel(), "My parameter to pass to view model");
s.Insert(1, new MyViewModel(), "My parameter to pass to view model");
```

Or with short cuts

```cs
var s = NavigationManager.GetSharedSource<MyViewModel>();
s.AddNew("My parameter to pass to view model");
s.InsertNew(1, "My parameter to pass to view model");
```

Remove

```cs
var s = NavigationManager.GetSharedSource<MyViewModel>();
s.RemoveAt(1);

var vieModel = new MyViewModel();
s.Remove(viewModel);
```

Clear

```cs
s.Clear();

s.ClearFast(); // without invoking ICanDeactivate and OnNavigatingFrom
```

Move

```cs
s.Move(1, 2); // moves the item at index 1 to index 2
```

Replace Item

```cs
var newItem = new MyViewModel();
NavigationHelper.Replace(1, newItem);
```

### Select an item

```cs
s.SelectedIndex = 1;

// or
var item = s.Items[1];
s.SelectedItem = item;
```

Note: The selected item is automatically set on insertion, deletion, etc. with **SelectionHandling** "Select" (default). Set the SelectionHandling to _None_ to remove this behavior.

```cs
s.SelectionHandling = SelectionHandling.None;
s.SelectionHandling = SelectionHandling.Select;
```

ViewModel Sample

```cs
public class ViewAViewModel: INavigationAware
{
    public SharedSource<MyItemDetailsViewModel> DetailsSource { get; }

    public SharedSourceSampleViewModel()
    {
        DetailsSource = NavigationManager.GetOrCreateSharedSource<MyItemDetailsViewModel>();
    }

    public void Load()
    {   
        // load data
        DetailsSource.Load(new List<MyItemDetailsViewModel>
        {
            new MyItemDetailsViewModel(new MyItem { Name = "Item.1" }),
            new MyItemDetailsViewModel(new MyItem { Name = "Item.2" })
        });
    }

    public void OnNavigatingFrom(NavigationContext navigationContext)
    {

    }

    public void OnNavigatingTo(NavigationContext navigationContext)
    {
        Load();
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {

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
<TabControl ItemsSource="{Binding DetailsSource.Items}"  
            SelectedItem="{Binding DetailsSource.SelectedItem}">
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

**Tip**: Use an interface (IDetailViewModel for example) for TabControls that can display multiple views.

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
  var viewFullName = viewType.FullName;
  viewFullName = viewFullName.Replace(".View.", ".ViewModel."); // <= 
  var suffix = viewFullName.EndsWith("View") ? "Model" : "ViewModel";
  var viewModelFullName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", viewFullName, suffix);
  var viewModelType = viewType.Assembly.GetType(viewModelFullName);

  return viewModelType;
});
```

### Register a custom View Model for a view

For example: the view model for a ViewA is not ViewAViewModel but MyCustomViewAViewModel

```cs
ViewModelLocationProvider.RegisterCustom(typeof(ViewA), typeof(MyCustomViewAViewModel));
```

### ResolveViewModel Attached property (Window, UserControl)

Allows to resolve the ViewModel (and inject dependencies) for the View. Usefull only for Views not used by the Navigation.

Example:


```xml
<Window x:Class="Sample.Views.Shell"
        ...
         xmlns:nav="http://mvvmlib.com/"
         nav:ViewModelLocator.ResolveViewModel="True">
```

**Note**: NavigationSources and Shared Sources resolve automatically the ViewModel with the ViewModelLocator. So, using "ResolveViewModel" attached property is rarely required.


**IIsLoaded** allows to notify view model that the view is loaded for a view that use resolve view model attached property or defined in Bootstrapper.

```cs
public class AuthorsViewModel : IIsLoaded
{
    private readonly IAuthorLookupService authorLookupService;

    public ObservableCollection<LookupItem> Authors { get; set; }

    public AuthorsViewModel(IAuthorLookupService authorLookupService)
    {
        this.authorLookupService = authorLookupService;
    }

    public async void LoadAsync()
    {
        var authors = await this.authorLookupService.GetAuthorLookupAsync();
        this.Authors = new ObservableCollection<LookupItem>(authors);
    }

    public void OnLoaded()
    {
        LoadAsync();
    }
}
```

## INavigationAware

> Allows to notify ViewModel with parameter

* **OnNavigatingFrom**
* **OnNavigatingTo**
* **OnNavigatedTo**

```cs
public class ViewAViewModel : INavigationAware
{
    public void OnNavigatingFrom(NavigationContext navigationContext)
    {

    }

    public void OnNavigatingTo(NavigationContext navigationContext)
    {
       // Usefull to preload data 
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        // gets the parameter
        var parameter = navigationContext.Parameter;
        // etc.
    }
}
```

The navigation context contains the navigation parameter. That allows to modify this parameter (on MoveToPrevious, MoveToNext, check activation, ect.). 

```cs
Navigation.Navigate(typeof(ViewA), "My parameter");
```

Use a dictionary for multiple parameters.

```cs
var navigationParameters = new Dictionary<string, object>
{
    { "redirectTo",typeof(ViewE) },
    { "parameter", navigationContext.Parameter }
};
Navigation.NavigateFast(typeof(LoginView), navigationParameters);
```

## Navigation Guards (ICanActivate, ICanDeactivate)

Useful when the user want to leave a page or close a tabitem for example.

```cs
public class ViewAViewModel : ICanActivate, ICanDeactivate
{

    public void CanActivate(NavigationContext navigationContext, Action<bool> continuationCallback)
    {
        var canActivate = MessageBox.Show("Can activate?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        continuationCallback(canActivate);
    }

    public void CanDeactivate(NavigationContext navigationContext, Action<bool> continuationCallback)
    {
        var canDeactivate = MessageBox.Show("Can deactivate?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        continuationCallback(canDeactivate);
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

## ListCollectionViewEx

> Allows to browse, filter, sort, group, add, edit with lists and collections.

```cs
People = new  List<Person>
{
    new Person { Id = 1, FirstName = "First1", LastName = "Last1" },
    new Person { Id = 2, FirstName = "First2", LastName = "Last2" },
    new Person { Id = 3, FirstName = "First3", LastName = "Last3"}
};

this.CollectionView = new ListCollectionViewEx(People);
```

Add buttons and bind commands

```xml
<!-- move -->
<Button Content="First" Command="{Binding CollectionView.MoveCurrentToFirstCommand}" />
<Button Content="Previous" Command="{Binding CollectionView.MoveCurrentToPreviousCommand}" />
<Button Content="Next" Command="{Binding CollectionView.MoveCurrentToNextCommand}" />
<Button Content="Last" Command="{Binding CollectionView.MoveCurrentToLastCommand}" />
<TextBox x:Name="RankTextBox" Text="{Binding CollectionView.Rank, Mode=OneWay}" Width="30" TextAlignment="Center" VerticalContentAlignment="Center" Margin="2">
    <mvvmLib:Interaction.Behaviors>
        <mvvmLib:EventToCommandBehavior EventName="KeyUp" 
                                        Command="{Binding CollectionView.MoveCurrentToRankCommand}"
                                        CommandParameter="{Binding ElementName=RankTextBox, Path=Text}"
                                        />
    </mvvmLib:Interaction.Behaviors>
</TextBox>
<TextBlock Text="{Binding CollectionView.Count, StringFormat='of {0}'}" VerticalAlignment="Center" Margin="5,0"/>

<!-- group -->
<Button Content="Group" Command="{Binding CollectionView.ToggleGroupByCommand}" CommandParameter="Type" />

<!-- sort -->
<Button Content="SORT (descending)" Command="{Binding CollectionView.SortByDescendingCommand}" CommandParameter="Name" />

<!-- Add edit delete -->
<Button Content="Add" Command="{Binding CollectionView.AddNewCommand}" />
<Button Content="Edit" Command="{Binding CollectionView.EditCommand}" />
<Button Content="Delete" Command="{Binding CollectionView.DeleteCommand}" />

<!-- cancel new or edit -->
<Button Content="Cancel" Command="{Binding CollectionView.CancelCommand}" />
<!-- commit new or commit edit -->
<Button Content="Save" Command="{Binding CollectionView.SaveCommand}" />
```

Filter

```cs
this.CollectionView.FilterBy<Person>(p => p.Age > age);
```
Reset filter

```cs
 this.CollectionView.ClearFilter();
```

Sort

```cs
this.CollectionView.SortBy("Age", true); // true to clear sort descriptions
```

Save

```cs
try
{
    if (this.CollectionView.IsAddingNew)
    {
        var current = this.CollectionView.CurrentAddItem as Person;
        current.ValidateAll();
        if (!current.HasErrors)
        {
            // save to db ...

            CollectionView.CommitNew();

            eventAggregator.GetEvent<NotificationMessageEvent>().Publish($"{current.FirstName} added!");
        }
    }
    else if (this.CollectionView.IsEditingItem)
    {
        var current = this.CollectionView.CurrentEditItem as Person;
        current.ValidateAll();
        if (!current.HasErrors)
        {

            // save to db ..

            CollectionView.CommitEdit();

            eventAggregator.GetEvent<NotificationMessageEvent>().Publish($"{current.FirstName} saved!");
        }
    }
}
catch (Exception ex)
{
    MessageBox.Show($"A problem occured:{ex.Message}");
}
```

Delete with confirmation

```cs
var current = this.CollectionView.CurrentItem as Person;
string name = current.FirstName;
var result = MessageBox.Show($"Delete {name}?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
if (result)
{
    try
    {
        // remove from db ...

        CollectionView.Remove(current);

        eventAggregator.GetEvent<NotificationMessageEvent>().Publish($"{name} removed!");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"A problem occured:{ex.Message}");
    }
}
```

Create new with injection (if an IoC Container is used for the SourceResolver factory)

```cs
var user = new UserWrapper(new User { RoleId = 1 });
this.CollectionView.AddNewItem(user);
```


## PagedSource

```cs
this.People = new ObservableCollection<Person>(peopleList);
this.PagedSource = new PagedSource(People, 10);
```

Filter

```cs
PagedSource.Filter = new Perdicate<object>(p => ((Person)p).Age > 30);
// or
PagedSource.FilterBy<Person>(p => p.Age > age);
   
// reset the list
PagedSource.Filter = null;
// or
PagedSource.ClearFilter();
```

Sort

```cs
PagedSource.CustomSort = new PersonSorter();
```

```cs
public class PersonSorter : IComparer
{
    public int Compare(object x, object y)
    {
        return ((Person)x).Age.CompareTo(((Person)y).Age);
    }
}
```

```xml
<DataGrid x:Name="DataGrid1" ItemsSource="{Binding PagedSource.Items}" AutoGenerateColumns="False" IsReadOnly="True" Grid.Row="1">
    <DataGrid.Columns>
        <DataGridTextColumn Header="First Name" Width="*" Binding="{Binding FirstName}" />
        <DataGridTextColumn Header="Last Name" Width="*" Binding="{Binding LastName}" />
        <DataGridTextColumn Header="Age" Width="*" Binding="{Binding Age}" />
    </DataGrid.Columns>
    <DataGrid.RowDetailsTemplate>
        <DataTemplate>
            <StackPanel Margin="10">
                <Border Height="160" Margin="0,5" BorderBrush="#eee" BorderThickness="2" HorizontalAlignment="Left">
                    <Image Source="{Binding ImagePath}" />
                </Border>
                <TextBlock Text="FIRSTNAME" Style="{StaticResource HeadingStyle}"/>
                <TextBlock Text="{Binding FirstName}" Margin="0 2 0 8"/>
                <TextBlock Text="LASTNAME" Style="{StaticResource HeadingStyle}"/>
                <TextBlock Text="{Binding LastName}" Foreground="Black" Margin="0 2 0 8"/>
            </StackPanel>
        </DataTemplate>
    </DataGrid.RowDetailsTemplate>
</DataGrid>
```

Commands

* MoveToFirstPageCommand
* MoveToPreviousPageCommand
* MoveToNextPageCommand
* MoveToLastPageCommand
* MoveToPageCommand with page index

Methods

* MoveToFirstPage
* MoveToPreviousPage
* MoveToNextPage
* MoveToLastPage
* MoveToPage with page index

Properties
* CanMoveToPreviousPage
* CanMoveToNextPage
* ItemsCount
* PageCount
* PageSize
* PageIndex
* CurrentPage (PageIndex + 1)
* Start
* End
* Items
* Filter
* CustomSorter

Events

* PropertyChanged
* Refreshed
* PageChanging
* PageChanged

## IIsSelected, ISelectable and SelectionSyncBehavior

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

**SelectionSyncBehavior** allows to notify all ViewModels (that implements IIsSelected) for a ListView with selection mode Multiple for example.

```xml
<ListView x:Name="ListView1" 
            ItemsSource="{Binding DetailsSource.Items}"
            SelectedItem="{Binding DetailsSource.SelectedItem}"
            SelectionMode="Multiple" 
            ItemContainerStyle="{StaticResource ListViewItemStyle}">
    <mvvmLib:NavigationInteraction.Behaviors>
        <mvvmLib:SelectionSyncBehavior />
    </mvvmLib:NavigationInteraction.Behaviors>
</ListView>
```

## AnimatableContentControl

> Content Control that allows to animate on content change. 

2 Storyboards : 

* EntranceAnimation 
* ExitAnimation
* Simultaneous (boolean) allows to play simultaneously the animations.
* CanAnimateOnLoad: allows to cancel animation on load

```xml
<mvvmLib:AnimatableContentControl mvvmLib:NavigationManager.SourceName="Main">
    <mvvmLib:AnimatableContentControl.EntranceAnimation>
        <Storyboard>
            <!-- Target "CurrentContentPresenter"  -->
            <DoubleAnimation Storyboard.TargetName="CurrentContentPresenter" 
                             Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"
                             From="400" To="0" Duration="0:0:0.4"  />
        </Storyboard>
    </mvvmLib:AnimatableContentControl.EntranceAnimation>
    <mvvmLib:AnimatableContentControl.ExitAnimation>
        <Storyboard>
            <!-- Target "CurrentContentPresenter" or with Simulatenous "PreviousContentPresenter" -->
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

## EventToMethodBehavior

Example: on button click, call the method "MyMethod" of the ViewModel (current DataContext) with a parameter

```xml
<Button Content="Click!" HorizontalAlignment="Left" Margin="5">
    <mvvmLib:Interaction.Behaviors>
        <mvvmLib:EventToMethodBehavior EventName="Click" TargetObject="{Binding}" MethodName="MyMethod" Parameter="MvvmLib!"/>
    </mvvmLib:Interaction.Behaviors>
</Button>
```

```cs
public class ViewAViewModel : BindableBase
{
    private string message;
    public string Message
    {
        get { return message; }
        set { SetProperty(ref message, value); }
    }

    private void MyMethod(object parameter)
    {
        Message = $"MyMethod invoked witth parameter '{parameter}' {DateTime.Now.ToLongTimeString()}";
    }
}
```

## Modules

> allows to manage modules/assemblies loaded "on demand"


### Registering modules

For example:

* Create a library "ModuleA"
* Add Views, ViewModels, etc.
* Create a module configuration file

```cs
using ModuleA.ViewModels;
using ModuleA.Views;
using MvvmLib.Modules;
using MvvmLib.Navigation;

namespace ModuleA
{
    public class ModuleAConfiguration : IModuleConfiguration
    {
        public void Initialize()
        {
            SourceResolver.RegisterTypeForNavigation<ViewA>(); // With View
            SourceResolver.RegisterTypeForNavigation<ViewBViewModel>("ViewB"); // With ViewModel (+ DataTemplate)
        }
    }
}
```

* Do not add a reference to this assembly from main project
* In main project. Register the module infos:

With Bootstrapper

Registering the module: 

* With ModuleManager.Default instance

```cs
public class Bootstrapper : MvvmLibBootstrapper
{
    //  etc.

    protected override void RegisterModules()
    {
        // 1. The module name (an id)
        // 2. The path
        // 3. The module config class full name
        ModuleManager.Default.RegisterModule("ModuleA", "ModuleA.dll", "ModuleA.ModuleAConfiguration")
    }
}
```

* Or Register a ModuleManager with IoC Container

```cs
public abstract class MvvmLibBootstrapper : BootstrapperBase
{
    protected IInjector container;

    public MvvmLibBootstrapper(IInjector container)
    {
        if (container == null)
            throw new ArgumentNullException(nameof(container));

        this.container = container;
    }

    protected override void RegisterRequiredTypes()
    {
        container.RegisterInstance<IInjector>(container);
        container.RegisterSingleton<IModuleManager, ModuleManager>(); // <=
        container.RegisterSingleton<IEventAggregator, EventAggregator>();
    }

    protected override void SetViewFactory()
    {
        SourceResolver.SetFactory((sourceType) => container.GetNewInstance(sourceType));
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
    // etc.

    protected override void RegisterModules()
    {
        var moduleManager = container.GetInstance<IModuleManager>();

        moduleManager.RegisterModule("ModuleA", @"ModuleA.dll", "ModuleA.ModuleAConfiguration");
    }
}
```

Or in App.Config file

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="modules" type="MvvmLib.Modules.ModulesConfigurationSection, MvvmLib.Wpf"/>
  </configSections>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
  </startup>
  <modules>
    <module moduleName="ModuleA" path="ModuleA.dll" moduleConfigurationFullName="ModuleA.ModuleAConfiguration"/>
  </modules>
</configuration>
```

**Tip**: copy dll to the main project (pre-build command lines in project properties)

```
copy "$(SolutionDir)Samples\Modules\ModuleA\bin\Debug\ModuleA.dll"  "$(ProjectDir)bin\Debug\ModuleA.dll" 
copy "$(SolutionDir)Samples\Modules\ModuleB\bin\Debug\ModuleB.dll"  "$(ProjectDir)bin\Debug\ModuleB.dll" 
```

### Loading the module

```cs
ModuleManager.Default.LoadModule("ModuleA");
```

or inject de module manager instance

```cs
public class ShellViewModel : BindableBase
{
    private readonly IModuleManager moduleManager;

    public NavigationSource Navigation { get; }
    public ICommand NavigateCommand { get; set; }


    public ShellViewModel(IModuleManager moduleManager)
    {
        this.moduleManager = moduleManager;

        this.Navigation = new NavigationSource();
        NavigateCommand = new RelayCommand<string>(NavigateToModule);

        moduleManager.ModuleLoaded += OnModuleLoaded;
    }

    private void OnModuleLoaded(object sender, ModuleLoadedEventArgs e)
    {

    }

    private void NavigateToModule(string sourceName)
    {
        var moduleName = GetModuleName(sourceName);
        if (moduleName == null)
            return;

        LoadModule(moduleName);

        this.Navigation.Navigate(sourceName);
    }

    private string GetModuleName(string sourceName)
    {
        switch (sourceName)
        {
            case "ViewA":
            case "ViewB":
                return "ModuleA";
            case "ViewC":
                return "ModuleB";
        }
        return null;
    }

    private void LoadModule(string moduleName)
    {
        if (!moduleManager.IsModuleLoaded(moduleName))
            moduleManager.LoadModule(moduleName);
    }
}
```

### Shared project (services, infrastructure, etc.)

Register the service with IoC container in main project

```cs
public class Bootstrapper : MvvmLibBootstrapper
{
    public Bootstrapper(IInjector container)
        : base(container)
    { }

    protected override void RegisterTypes()
    {
        // shared service
        container.RegisterType<IMySharedService, MySharedService>();
    }

    // etc.
}
```

Use this service in modules

```cs
public class ViewCViewModel : BindableBase
{
    public ViewCViewModel(IMySharedService mySharedService)
    {
        
    }
}
```


