# Mvvm

* **BindableBase**, **Editable**, **Validatable**, **ValidatableAndEditable** and **ModelWrapper** base classes for _Models and ViewModels_
* **NotifyPropertyChangedObserver** and **FilterableNotifyPropertyChangedObserver**: allows to observe and filter an object that implements INotifyPropertyChanged
* **Commands** and **composite** command
* **Sync** _extensions for list and collections_. Allows to **sync data**
* **Singleton**
* **EventAggregator** : allows to **subscribe**, **publish** and filter messages

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
            OnPropertyChanged("FullName");
        }
    }

    private string lastName;
    public string LastName
    {
        get { return lastName; }
        set
        {
            SetProperty(ref lastName, value);
            OnPropertyChanged("FullName");
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

> Allows to validate.

## ValidatableAndEditable

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
public class User : ValidatableAndEditable
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
user.ValidateProperty("FirstName", "");

// validate all
user.ValidateAll();

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

        SaveCommand = new RelayCommand(OnSave);
        ResetCommand = new RelayCommand(OnReset);
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

## NotifyPropertyChangedObserver and FilterableNotifyPropertyChangedObserver

> Allows to observe and filter an object that implements INotifyPropertyChanged

Example 1:

```cs
var owner = new User();
var propertyName = "FirstName";

// the callback is invoked only if the property name is "FirstName" for example
var filter = new Func<INotifyPropertyChanged, PropertyChangedEventArgs, bool>((s, e) => e.PropertyName == propertyName);

var propertyChangedObserver = new FilterableNotifyPropertyChangedObserver(owner, filter);
propertyChangedObserver.SubscribeToPropertyChanged((e) => RaiseCanExecuteChanged());
```

Example 2:
The owner can be any object that implements INotifyPropertyChanged. Example with the ItemCollection of a TabControl. Notification on tab item count changed.

```cs
var observer = new NotifyPropertyChangedObserver(MyTabControl.Items);
observer.SubscribeToPropertyChanged((e) =>
{
    MessageBox.Show(e.PropertyName);
});
```

We can use PropertyDescriptor in Wpf.

```cs
DependencyPropertyDescriptor
    .FromProperty(TabControl.HasItemsProperty, typeof(TabControl))
    .AddValueChanged(MyTabControl, (s, e) =>
    {
        MessageBox.Show(e.ToString()); // raised when the tab items collection is empty
    });
```

## Relay command

Example

```cs
public class PageAViewModel : ViewModel
{
        public IRelayCommand MyCommand { get; }

        public PageAViewModel()
        {     
            this.MyCommand = new RelayCommand(() => {  /* do something */ });
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
            this.MyCommand = new RelayCommand<string>((value) =>{  /* do something */ }, (value) => true);
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

    public IRelayCommand SaveCommand { get; set; }

    public TabViewModel()
    {
        canSave = true;
        SaveCommand = new RelayCommand(OnSave, CheckCanSave);
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

    public IRelayCommand SaveCommand { get; set; }

    public TabViewModel()
    {
        canSave = true;
        SaveCommand = new RelayCommand(OnSave, CheckCanSave)
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


## Composite command

The command is executed only if all conditions are true

```cs
// create a composite command
var compositeCommand = new CompositeCommand();

// create commands
var commandA = new RelayCommand(() => {  /* do something */ });
var commandB = new RelayCommand(() => {  /* do something */ });

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

    public IRelayCommand SaveCommand { get; set; }

    public TabViewModel(IApplicationCommands applicationCommands)
    {
        canSave = true;
        SaveCommand = new RelayCommand(OnSave, CheckCanSave);

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
public class ShellViewModel : ILoadedEventListener
{
    public CompositeCommand SaveAllCommand { get; }

    private IRegionManager regionManager;

    public ShellViewModel(IApplicationCommands applicationCommands, IRegionManager regionManager)
    {
        SaveAllCommand = applicationCommands.SaveAllCommand;
        this.regionManager = regionManager;
    }

    public async void OnLoaded(object parameter)
    {
        // create 3 tabs for a tab control
        await regionManager.GetItemsRegion("TabRegion").AddAsync(typeof(TabView), "TabA");
        await regionManager.GetItemsRegion("TabRegion").AddAsync(typeof(TabView), "TabB");
        await regionManager.GetItemsRegion("TabRegion").AddAsync(typeof(TabView), "TabC");
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

Call **Sync** \(extension method for lists and collections\)

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

## Singleton

Example:

```cs
Singleton<MyService>.Instance.DoSomething();
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
```

