## MvvmLib.Adaptive.Windows [uwp]
  
* **BreakpointBinder**: allows to **bind controls** and make the page "**responsive**"
* **BreakpointListener**: allows to define a list of breakpoints and be notified on current breakpoint changed
* **ResponsiveGridView** and **ResponsiveVariableSizedGridView**
* **BusyIndicator**


## BreakpointListener

Example:
```cs
var listener = new BreakpointListener();

// register breakpoints
listener.AddBreakpoint(0);
listener.AddBreakpoint(320);
listener.AddBreakpoint(640);
listener.AddBreakpoint(960);
listener.AddBreakpoint(1280);
listener.AddBreakpoint(1600);

listener.BreakpointChanged += (s, e) =>
{
      // notified on active breakpoint changed
      // e.Width
};
```

## BreakpointBinder

### DataContext (code-behind or ViewModel)

Example:

```cs
public class MyViewModel : ViewModelBase
{
        private Dictionary<string, object> active;
        public Dictionary<string, object> Active
        {
            get { return active; }
            set { Set(ref this.active, value); }
        }

        private void OnActiveChanged(object sender, ActiveChangedEventArgs e)
        {
            this.Active = e.Active;
        }

        public override async void OnNavigatedTo(object parameter)
        {
            var binder = new BreakpointBinder();

            // with dictionary string : object
            binder.AddBreakpointWithBindings(0, new Dictionary<string, object> { { "TitleFontSize", "14" }, { "TitleColor", "Green" } });

            // with Adaptive container helper
            binder.AddBreakpointWithBindings(640, AdaptiveContainer.Create().Set("TitleFontSize", "80").Set("TitleColor", "Red").Get());
            binder.AddBreakpointWithBindings(960, AdaptiveContainer.Create().Set("TitleFontSize", "160").Set("TitleColor", "Blue").Get());

            // deferred
            // await Task.Delay(4000);

            binder.ActiveChanged += OnActiveChanged;
        }
}
```

Define the **DataContext** of the view to the **viewModel**

And bind controls to the Active dictionary values

```xml
<TextBlock Text="My adaptive text" 
           FontSize="{Binding Active.TitleFontSize}" 
           Foreground="{Binding Active.TitleColor}"></TextBlock>
```

### Control

Add to the view the control and load for example from json file

_In this example, the font size and the foreground change with the window size_

```xml
<!-- Control -->
<Adaptive:BreakpointBinder x:Name="bp" File="Breakpoints/AsControlView.json" />
        
<TextBlock Text="My adaptive text" 
           FontSize="{Binding ElementName=bp,Path=Active.TitleFontSize}" 
           Foreground="{Binding ElementName=bp,Path=Active.TitleColor}"></TextBlock>
```

Define the **json file** as **content** in properties panel

```json
[
  {
    "minwidth": 0,
    "bindings": {
      "TitleFontSize": "20",
      "TitleColor": "red"
    }
  },
  {
    "minwidth": 640,
    "bindings": {
      "TitleFontSize": "50",
      "TitleColor": "blue"
    }
  },
  {
    "minwidth": 960,
    "bindings": {
      "TitleFontSize": "100",
      "TitleColor": "green"
    }
  }
]
```

Namespace
```
xmlns:adaptive="using:UwpLib.Adaptive"
```

## Responsive GridView

```xml
<adaptive:ResponsiveGridView x:Name="NormalGridView"
                                     Columns="6"
                                     ItemHeight="200"
                                     ItemsSource="{Binding ElementName=ThisPage,Path=Items,Mode=OneWay}"
                                     ItemTemplate="{StaticResource SquareTemplate}"
                                     ItemClickCommand="{Binding ElementName=ThisPage,Path=Command}" Grid.Row="1">
 </adaptive:ResponsiveGridView>
```               

## One row mode

```xml
<adaptive:ResponsiveGridView x:Name="OneRowModeGridView"
                                     OneRowMode="True"
                                     Columns="3"
                                     ItemHeight="300"
                                     ItemsSource="{Binding ElementName=ThisPage,Path=Items,Mode=OneWay}"
                                     ItemTemplate="{StaticResource SquareTemplate}"
                                     ItemClickCommand="{Binding ElementName=ThisPage,Path=Command}" Grid.Row="1">
</adaptive:ResponsiveGridView>
```

### With Breakpoints

```xml
<adaptive:BreakpointBinder x:Name="bp" File="path/to/breakpoints.json"/>

<adaptive:ResponsiveGridView x:Name="ResponsiveGridView"                                        
                                         ItemsSource="{Binding ElementName=ThisPage,Path=Items,Mode=OneWay}"
                                         ItemTemplate="{StaticResource SquareTemplate}"
                                         ItemClickCommand="{Binding ElementName=ThisPage,Path=Command}" 
                                         Columns="{Binding Active.columns, ElementName=bp,FallbackValue=4}"
                                         ItemHeight="300"
                                         Grid.Row="1">
</adaptive:ResponsiveGridView>
```
The Json file

```json
[
    {
        "minwidth": 0,
        "bindings": {
            "columns": 2
        }
    },
    {
        "minwidth": 600,
        "bindings": {
            "columns": 3
        }
    },
    {
        "minwidth": 960,
        "bindings": {
            "columns": 4
        }
    }
]
```

## Responsive variable sized GridView

```xml
 <ScrollViewer>
            <adaptive:ResponsiveVariableSizedGridView x:Name="GridView"
            Columns="6"
            ItemHeight="150"
            ItemsSource="{Binding ElementName=ThisPage,Path=Items}">
                <adaptive:ResponsiveVariableSizedGridView.ItemTemplate>
                    <DataTemplate>
                        <Grid Background="{Binding Color}"  Margin="4">
                            <TextBlock Text="{Binding Title}"
                                       HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Grid>
                    </DataTemplate>
                </adaptive:ResponsiveVariableSizedGridView.ItemTemplate>
            </adaptive:ResponsiveVariableSizedGridView>
</ScrollViewer>
```

Items source have to inherit from "VariableSizedGridViewItem". It allows to define ColumnSpan and RowSpan for each item.
```cs
public class VariableItem : VariableSizedGridViewItem
{
        public string Title { get; set; }
        public string Color { get; set; }
}
```

Example for items source
```cs
 var result = new List<VariableItem>()
{
                new VariableItem { ColumnSpan=2,RowSpan=2,Color="DarkRed", Title="Item 1" },
                new VariableItem { ColumnSpan=1,RowSpan=1,Color="DarkGray", Title="Item 2" },
                new VariableItem { ColumnSpan=1,RowSpan=1,Color="Brown", Title="Item 3" },
                new VariableItem { ColumnSpan=2,RowSpan=2,Color="DarkGreen", Title="Item 4" },
                new VariableItem { ColumnSpan=1,RowSpan=1,Color="DarkOrange", Title="Item 5" },
                new VariableItem { ColumnSpan=1,RowSpan=1,Color="DarkCyan", Title="Item 6" },
};
```

### With breakpoints

```xml
<adaptive:BreakpointBinder x:Name="bp" File="path/to/breakpoints.json"/>

 <adaptive:ResponsiveVariableSizedGridView x:Name="ResponsiveVariableSizedGridView"
                                                      Columns="{Binding Active.columns, ElementName=bp,FallbackValue=6}"
                                                      ItemHeight="150"
                                                      ItemsSource="{Binding ElementName=ThisPage,Path=VariableSizedItems,Mode=OneWay}"
                                                      ItemTemplate="{StaticResource SquareTemplate}" Grid.Row="1">
</adaptive:ResponsiveVariableSizedGridView>
```

The Json file

```json
[
    {
        "minwidth": 0,
        "bindings": {
            "columns": 2
        }
    },
    {
        "minwidth": 600,
        "bindings": {
            "columns": 4
        }
    },
    {
        "minwidth": 960,
        "bindings": {
            "columns": 6
        }
    }
]
```
![](http://res.cloudinary.com/romagny13/image/upload/v1515717015/resizing3_iubnqj.png)


### DataTemplateSelector

Responsive GridViews support DataTemplateSelectors.


## Busy Indicator

```xml
<Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition />
    </Grid.RowDefinitions>

    <!-- content -->

    <adaptive:BusyIndicator x:Name="BusyIndicator" Grid.RowSpan="2"></adaptive:BusyIndicator>
    
</Grid>
```

```cs
this.BusyIndicator.IsActive = true;

await Task.Delay(2000);

this.BusyIndicator.IsActive = false;
```
