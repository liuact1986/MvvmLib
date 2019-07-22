# Mvvm

* **BindableBase**, **Editable**, **Validatable** and **ModelWrapper** base classes for _Models and ViewModels_
* **DelegateCommand**, **AsyncCommand** and **CompositeCommand**
* **EventAggregator** : allows to **subscribe**, **publish** and filter messages
* **ChangeTracker**: allows to track object changes.
* **Singleton**
* **SyncUtils** Allows to **sync lists and collections** 

## BindableBase

> Implements _INotifyPropertyChanged_ interface.

Allows to observe a property and notify the view that a value has changed.

SetProperty

```cs
public class UserViewModel : BindableBase
{
    private string firstName;
    public string FirstName
    {
        get { return firstName; }
        set { SetProperty(ref firstName, value); }
    }
}
```

OnPropertyChanged

```cs
public class UserViewModel : BindableBase
{
    private string firstName;
    public string FirstName
    {
        get { return firstName; }
        set
        {
            SetProperty(ref firstName, value);
            OnPropertyChanged(nameof(FullName));
        }
    }

    private string lastName;
    public string LastName
    {
        get { return lastName; }
        set
        {
            SetProperty(ref lastName, value);
            OnPropertyChanged(nameof(FullName));
        }
    }

    public string FullName
    {
        get { return $"{firstName} {LastName}"; }
    }
}
```

Or with **Linq** Expression

```cs
public class User : BindableBase
{

    private string firstName;
    public string FirstName
    {
        get { return firstName; }
        set
        {
            firstName = value;
            OnPropertyChanged(() => FirstName);
        }
    }
}
```

## Editable 

> Implements _IEditableObject_ interface.

Allows to cancel and **restore** **old values**. Entity Framework (ICollection, ect.) and circular references are supported.

| Method | Description |
| --- | --- |
|  BeginEdit | Store model values |
|  CancelEdit | Restore model values |

Example

```cs
public class User : Editable
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    // object, list , etc.
}
```

```cs
var user = new User
{
    FirstName = "Marie",
    LastName = "Bellin",
    // etc.
};

user.BeginEdit();

user.FirstName = "Marie!!";

user.CancelEdit(); 
```

## Validatable

> Validation + Edition

Allows to **validate** the model with **Data Annotations** and **custom validations**. Allows to cancel and **restore** **old values**. 

| Validation Handling | Description |
| --- | --- |
|  OnPropertyChange | Default. Validation on property changed |
|  OnSubmit | Validation only after "ValidateAll" invoked. Validation on property changed only after "ValidateAll" invoked |
|  Explicit | Validation only with "ValidateProperty" and "ValidateAll" |


| Property | Description |
| --- | --- |
|  UseDataAnnotations | Ignore or use Data Annotations for validation  |
|  UseCustomValidations | Ignore or use Custom validations |


| Method | Description |
| --- | --- |
|  ValidateProperty | Validate one property  |
|  ValidateAll | Validate all properties |
|  Reset | Reset the errors and is submitted |
|  BeginEdit | Store model values |
|  CancelEdit | Restore model values and errors |

The model requires to use SetProperty

```cs
public class User : Validatable
{
    private string firstName;

    [Required]
    [StringLength(50)]
    public string FirstName
    {
        get { return firstName; }
        set { SetProperty(ref firstName, value); }
    }

    // object, list , etc.
}
```

```cs
var user = new User
{
    FirstName = "Marie",
    LastName = "Bellin",
    // etc.
};


// validate a property
user.ValidateProperty("FirstName");

// validate all
user.ValidateAll();

// summary
var allErrors = user.GeErrorSummary();

if (user.HasErrors)
{

}

// reset the errors and is submitted
user.Reset();

// reset the errors, is submitted and the model
user.CancelEdit();

// etc.
```

## ModelWrapper

> Allows to wrap, edit and validate a model.

The model does not require to use SetProperty

```cs
public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [StringLength(50)]
    public string LastName { get; set; }

    // object, list , etc.
}
```

Create a Generic model wrapper class

```cs
public class UserWrapper : ModelWrapper<User>
{
    public UserWrapper(User model) : base(model)
    {
    }

    public int Id { get { return Model.Id; } }

    public string FirstName
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    public string LastName
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    // etc.

    // custom validations
    protected override IEnumerable<string> DoCustomValidations(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(FirstName):
                if (string.Equals(FirstName, "Marie", StringComparison.OrdinalIgnoreCase))
                {
                    yield return "Marie is not allowed";
                }
                break;
        }
    }
}
```

ViewModel sample

```cs
public class UserDetailViewModel : BindableBase
{
    private UserWrapper user;
    public UserWrapper User
    {
        get { return user; }
        set { SetProperty(ref user, value); }
    }

    public ICommand SaveCommand { get; }
    public ICommand ResetCommand { get; }

    public UserDetailViewModel()
    {
        User = new UserWrapper(new Models.User
        {
            Id = 1
        });

        this.user.BeginEdit();

        SaveCommand = new DelegateCommand(OnSave);
        ResetCommand = new DelegateCommand(OnReset);
    }

    private void OnSave()
    {
        this.User.ValidateAll();
    }

    private void OnReset()
    {
        this.user.CancelEdit(); // reset the errors and the model
    }
}
```

**Wpf**

Binding

```xml
<!-- the default value of UpdateSourceTrigger is LostFocus -->
<TextBox Text="{Binding User.FirstName, UpdateSourceTrigger=PropertyChanged}" />
```

Create a Style that displays errors

```xml
<Style TargetType="TextBox">
    <Setter Property="Validation.ErrorTemplate">
        <Setter.Value>
            <ControlTemplate>
                <StackPanel>
                    <AdornedElementPlaceholder x:Name="placeholder"/>
                    <!--TextBlock with error -->
                    <TextBlock FontSize="12" Foreground="Red" 
                               Text="{Binding ElementName=placeholder,Path=AdornedElement.(Validation.Errors)[0].ErrorContent}"/>
                </StackPanel>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
    <Style.Triggers>
        <Trigger Property="Validation.HasError" Value="True">
            <Setter Property="Background" Value="Red"/>
            <!--Tooltip with error -->
            <Setter Property="ToolTip" 
                    Value="{Binding RelativeSource={RelativeSource Self}, Path=(Validation.Errors)[0].ErrorContent}"/>
        </Trigger>
    </Style.Triggers>
</Style>
```

**Uwp**

```xml
<TextBox Text="{Binding User.FirstName, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
<TextBlock Text="{Binding User.Errors[FirstName][0]}" Foreground="Red"></TextBlock>
```

Or Create a _Converter_ that displays the _first error_ of the list

```cs
public sealed class FirstErrorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var errors = value as IList<string>;
        return errors != null && errors.Count > 0 ? errors.ElementAt(0) : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
```

And use it
```xml
<Page.Resources>
    <converters:FirstErrorConverter x:Name="FirstErrorConverter"></converters:FirstErrorConverter>
</Page.Resources>
```

```xml
 <TextBox Text="{Binding User.FirstName, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
<!-- with converter -->
<TextBlock Text="{Binding User.Errors[FirstName], Converter={StaticResource FirstErrorConverter}}" Foreground="Red"></TextBlock>
```

## DelegateCommand

Example

```cs
public class PageAViewModel : ViewModel
{
        public IDelegateCommand MyCommand { get; }

        public PageAViewModel()
        {     
            this.MyCommand = new DelegateCommand(() => {  /* do something */ });
        }
}
```

And in the view

```xml
<Button Command="{Binding MyCommand}">Do something</Button>
```

Other example **with parameter and condition**

```cs
public class PageAViewModel
{
    public ICommand MyCommand { get; }

    public PageAViewModel()
    {     
        this.MyCommand = new DelegateCommand<string>((value) =>{  /* do something */ }, (value) => true);
    }
}
```

```xml
<Button Command="{Binding MyCommand}" CommandParameter="my parameter">Do something</Button>
```

**RaiseCanExecuteChanged** allows to invoke the can execute method "on demand".

```cs
public class TabViewModel : BindableBase
{
    private bool canSave;
    public bool CanSave
    {
        get { return canSave; }
        set
        {
            if (SetProperty(ref canSave, value))
            {
                SaveCommand.RaiseCanExecuteChanged(); // <=
            }
        }
    }

    public IDelegateCommand SaveCommand { get; set; }

    public TabViewModel()
    {
        canSave = true;
        SaveCommand = new DelegateCommand(OnSave, CheckCanSave);
    }

    private void OnSave()
    {
        var message = $"Save TabView {Title}! {DateTime.Now.ToLongTimeString()}";
        MessageBox.Show(message);
        SaveMessage = message;
    }

    private bool CheckCanSave()
    {
        return canSave;
    }  
}
```

Or with **ObserveProperty**. This method allows to **observe a property** and **raise can execute changed automatically** on property changed. 

```cs
public class TabViewModel : BindableBase
{
    private bool canSave;
    public bool CanSave
    {
        get { return canSave; }
        set { SetProperty(ref canSave, value); }
    }

    public IDelegateCommand SaveCommand { get; set; }

    public TabViewModel()
    {
        canSave = true;
        SaveCommand = new DelegateCommand(OnSave, CheckCanSave)
            .ObserveProperty(() => CanSave); // <=
    }

    private void OnSave()
    {
        var message = $"Save TabView {Title}! {DateTime.Now.ToLongTimeString()}";
        MessageBox.Show(message);
        SaveMessage = message;
    }

    private bool CheckCanSave()
    {
        return canSave;
    }  
}
```

## AsyncCommand


Example

```cs
public class MainWindowViewModel : BindableBase
{
    private bool isBusy;
    public bool IsBusy
    {
        get { return isBusy; }
        set { SetProperty(ref isBusy, value); }
    }

    private IAsyncCommand myCommand;
    public IAsyncCommand MyCommand
    {
        get
        {
            if (myCommand == null)
                myCommand = new AsyncCommand(ExecuteMyCommand);
            return myCommand;
        }
    }

    private async Task ExecuteMyCommand()
    {
        IsBusy = true;

        await Task.Delay(3000); // do some work

        IsBusy = false;
    }
}
```

Supports cancellation:

```cs
myCommand.Cancel(); 
// other sample 
myCommand.CancellationTokenSource.CancelAfter(500);
```

Checks cancellation

```cs
if (myCommand.IsCancellationRequested)
    return;
```

Handle exception

```cs
myCommand = new AsyncCommand(ExecuteMyCommand, ex => { });
```

## Composite command

The command is executed only if all conditions are true

```cs
// create a composite command
var compositeCommand = new CompositeCommand();

// create commands
var commandA = new DelegateCommand(() => {  /* do something */ });
var commandB = new DelegateCommand(() => {  /* do something */ });

// add commands to the composite command
compositeCommand.Add(commandA);
compositeCommand.Add(commandB);

// the composite command executes all registered commands if all commands can execute
compositeCommand.Execute(null);
```

_Real sample_

Create an interface and a class with a composite command. This allows to inject the service to view models with an IoC container.

```cs
public interface IApplicationCommands
{
    CompositeCommand SaveAllCommand { get; }
}

public class ApplicationCommands : IApplicationCommands
{
    public CompositeCommand SaveAllCommand { get; } = new CompositeCommand();
}
```

Register with MvvmLib.IoC Container at App Startup for example:

```cs
 container.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
```
Create a view and view model (for a tabcontrol for example)

```cs
public class TabViewModel : BindableBase, INavigatable
{
    // other properties, etc.

    private bool canSave;
    public bool CanSave
    {
        get { return canSave; }
        set
        {
            if (SetProperty(ref canSave, value))
            {
                // the composite is notified that can execute of this command changed (enable/disable the SaveAllCommand button)
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public IDelegateCommand SaveCommand { get; set; }

    public TabViewModel(IApplicationCommands applicationCommands)
    {
        canSave = true;
        SaveCommand = new DelegateCommand(OnSave, CheckCanSave);

        // add the command to the composite command
        applicationCommands.SaveAllCommand.Add(SaveCommand);
    }

    private void OnSave()
    {
        var message = $"Save TabView {Title}! {DateTime.Now.ToLongTimeString()}";
        MessageBox.Show(message);
        SaveMessage = message;
    }

    private bool CheckCanSave()
    {
        return canSave;
    }

    // INavigatable implementation, etc.
}
```

Bind the command in the view

```xml
<UserControl x:Class="CompositeCommandSample.Views.TabView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
             xmlns:local="clr-namespace:CompositeCommandSample.Views"
             mc:Ignorable="d" 
             d:DesignHeight="450" d:DesignWidth="800">

    <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
        <TextBlock Text="{Binding Title}" FontSize="18" FontWeight="Bold" Margin="4"></TextBlock>
        <CheckBox IsChecked="{Binding CanSave}" Content="Can save?" Margin="4"></CheckBox>
        <!-- the button is disabled if cannot save -->
        <Button Content="Save" Command="{Binding SaveCommand}" Margin="4"></Button>
        <TextBlock Text="{Binding SaveMessage}" Margin="4"></TextBlock>
    </StackPanel>

</UserControl>
```

Create the shell and the ShellViewModel (Sample with Wpf)

```cs
public class ShellViewModel
{
    private readonly IApplicationCommands applicationCommands;

    public CompositeCommand SaveAllCommand { get; }
    public SharedSource<TabViewModel> TabItemsSource { get; }

    public ShellViewModel(IApplicationCommands applicationCommands)
    {
        this.applicationCommands = applicationCommands;
        SaveAllCommand = applicationCommands.SaveAllCommand;

        this.TabItemsSource = NavigationManager.GetOrCreateSharedSource<TabViewModel>();
        this.Load();
    }

    public void Load()
    {
        this.TabItemsSource.Items.Add(new TabViewModel(applicationCommands, "TabA"));
        this.TabItemsSource.Items.Add(new TabViewModel(applicationCommands, "TabB"));
        this.TabItemsSource.Items.Add(new TabViewModel(applicationCommands, "TabC"));
    }
}
```

Bind the composite command in the Shell

```xml
<Window x:Class="CompositeCommandSample.Views.Shell"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:CompositeCommandSample.Views"
        xmlns:nav="http://mvvmlib.com/"
        nav:ViewModelLocator.ResolveWindowViewModel="True"
        mc:Ignorable="d"
        Title="Shell" Height="450" Width="800">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition />
        </Grid.RowDefinitions>

        <!-- the button is disabled if one command cannot save -->
        <Button Content="Save All" Command="{Binding SaveAllCommand}" Width="120" Margin="4" HorizontalAlignment="Left"></Button>

        <TabControl nav:RegionManager.ItemsRegionName="TabRegion" Grid.Row="1">
            <TabControl.ItemContainerStyle>
                <Style TargetType="TabItem">
                    <Setter Property="Header" Value="{Binding Title}" />
                </Style>
            </TabControl.ItemContainerStyle>
        </TabControl>
        
    </Grid>
</Window>
```


## ChangeTracker

> Usefull for example with a TabItem with an asterisk that indicates changes. Allows to track changes for list/ collections or Objects.

Properties:

* HasChanges: updated when the source has changed.

Methods:

* TrackChanges
* CheckChanges for all or only for a property
* AcceptChanges

```cs
public class Person : BindableBase
{
    private string firstName;
    public string FirstName
    {
        get { return firstName; }
        set { SetProperty(ref firstName, value); }
    }

    private string lastName;
    public string LastName
    {
        get { return lastName; }
        set { SetProperty(ref lastName, value); }
    }

    // or ObservableCollection, etc.
    public List<Friend> Friends { get; set; }

    public Person()
    {
        this.Friends = new List<Friend>();
    }
}

public class Friend : BindableBase
{
    private string email;
    public string Email
    {
        get { return email; }
        set { SetProperty(ref email, value); }
    }
}
```

```cs
this.Person = new Person();

this.Tracker = new ChangeTracker();
this.Tracker.TrackChanges(this.Person);
```

For list or collection, create a command and invoke this.Tracker.CheckChanges() or observe INotifyCollectionChanged for ObservableCollection for example.

```cs
private void AddFriend(Friend friend)
{
    this.Person.Friends.Add(friend);
    this.Tracker.CheckChanges();
}
```

TabControl with asterisk for example

```xml
<TabControl ItemsSource="{Binding TabItemsSource.Items}" 
            SelectedItem="{Binding TabItemsSource.SelectedItem}" 
            Grid.Row="1">
    <TabControl.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="{Binding Title}" Margin="10,0"/>
                <TextBlock Text="*" VerticalAlignment="Top"
                            Visibility="{Binding Tracker.HasChanges, Converter={StaticResource BooleanToVisibilityConverter}}"/>
            </StackPanel>
        </DataTemplate>
    </TabControl.ItemTemplate>
</TabControl>
```

## Singleton

Example:

```cs
Singleton<MyService>.Instance.DoSomething();
```

## Sync Data

Model have to implement **ISyncItem** interface:

* **NeedSync**: check if the "**other**" instance is the same \("**deep equal**"\)
* **Sync** : **update values** **with **the "**other**" instance** values**

```cs
public class Item : BindableBase, ISyncItem<Item>
{
    public string Id { get; set; }

    private string _title;
    public string Title
    {
        get { return _title; }
        set { this.SetProperty(ref _title, value); }
    }

    // other properties ...

    public bool NeedSync(Item other)
    {
        return Id == other.Id && (this.Title != other.Title); // Testing for each property
    }

    public void Sync(Item other)
    {
        Title = other.Title;
        // etc.
    }

    public bool Equals(Item other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (ReferenceEquals(null, other)) return false;

        return this.Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Item);
    }

    public override int GetHashCode()
    {
        if (string.IsNullOrEmpty(this.Id))
            return 0;
            
        return this.Id.GetHashCode();
    }
}
```

Example:

```cs
 var oldItems = new List<Item> {
                new Item { Id="1", Title = "Title 1" },
                new Item { Id="2", Title = "Title 2" },
                new Item { Id="3", Title = "Title 3" }
            };

var newItems = new List<Item>{
                new Item{ Id="2", Title = "Title 2!!!!!!"},
                new Item{ Id="3", Title = "Title 3"},
                new Item{ Id="4", Title = "Title 4"}
            };

SyncUtils.Sync(oldItems, newItems);
// item 1 removed
// item 2 updated
// item 3 not updated
// item 4 added
```


## EventAggregator

> allows to subscribe, publish and filter messages


Register the eventAggregator as singleton with an ioc container. 

```cs
public class ShellViewModel : BindableBase
{
    public ShellViewModel(EventAggregator eventAggregator)
    {

    }
}
```

Or without an ioc container, use the Singleton class.
```cs
var eventAggregator = Singleton<EventAggregator>.Instance;
```

**Subscribe** and **publish** with empty event (no parameter)

Create the event class
```cs
public class MyEvent : EmptyEvent
{ }
```

```cs
// subscriber
eventAggregator.GetEvent<MyEvent>().Subscribe(() => { /* do something with args.Message */ });
```

```cs
// publisher
eventAggregator.GetEvent<MyEvent>().Publish();
```

**Subscribe** and **publish** with parameter

```cs
public class DataSavedEvent : ParameterizedEvent<DataSavedEventArgs>
{ }

public class DataSavedEventArgs
{
    public string Message { get; set; }
}
```

```cs
// subscriber
eventAggregator.GetEvent<DataSavedEvent>().Subscribe(args => { /* do something with args.Message */ });
```

```cs
// publisher
eventAggregator.GetEvent<DataSavedEvent>().Publish(new DataSavedEventArgs { Message = "Data saved." })
```

**Filter**

Example: Filter on "user id"

```cs
eventAggregator.GetEvent<MyUserEvent>().Subscribe(user => { /* do something */ }).WithFilter(user => user.Id == 1); // <= not notified

messenger.GetEvent<MyUserEvent>().Publish(new User { Id = 2, UserName = "Marie" });  // <=
```
The event class:

```cs
public class MyUserEvent : ParameterizedEvent<User>
{ }
```

**Execution strategy**:

* **PublisherThread** (default)
* **UIThread**
* **BackgroundThread**

```cs
eventAggregator.GetEvent<DataSavedEvent>().Subscribe(_ => { }).WithExecutionStrategy(ExecutionStrategyType.UIThread);
```

**Unsubscribe** with the token received on subscription.

```cs
var subscriberOptions = eventAggregator.GetEvent<DataSavedEvent>().Subscribe(_ => { });

var success = eventAggregator.GetEvent<DataSavedEvent>().Unsubscribe(subscriberOptions.Token);
// or
subscriberOptions.Unsubscribe();
```