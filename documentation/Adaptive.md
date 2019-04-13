## MvvmLib.Adaptive.Wpf [net 4.5]

* **BreakpointBinder**: allows to **bind controls** and make the page "**responsive**"
* **BreakpointListener**: allows to define a list of breakpoints and be notified on current breakpoint changed

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
           FontSize="{Binding Active[TitleFontSize]}" 
           Foreground="{Binding Active[TitleColor]}"></TextBlock>
```

### Control

Add to the view the control and load for example from json file

_In this example, the font size and the foreground change with the window size_

```xml
<!-- Control -->
<Adaptive:BreakpointBinder x:Name="bp" File="Breakpoints/AsControlView.json" />
        
<TextBlock Text="My adaptive text" 
           FontSize="{Binding ElementName=bp,Path=Active[TitleFontSize]}" 
           Foreground="{Binding ElementName=bp,Path=Active[TitleColor]}"></TextBlock>
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
xmlns:Adaptive="http://wpflib.com/" 
```