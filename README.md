# MvvmLib 0.0.1

>  Mvvm Library for .NET projects.


* **MvvmLib.Core** [netstandard 2.0, net 4.5]
  * **BindableBase**, **Validatable**, **Editable**, **ValidateAndEditable** and **ModelWrapper** base classes for _Models and ViewModels_
  * **Commands** and **composite** command
  * **Sync** _extensions for list and collections_. Allows to **sync data**
  * **Singleton**
  * **Messenger** : allows to **subscribe**, **publish** and filter messages



* **MvvmLib.IoC** (Dependency Injection) [netstandard 2.0, net 4.5]
  * **Injector**: allows to **register Types, Singletons, Instances, values, auto discover types, etc.**.
  


* **MvvmLib.Wpf** (Navigation) [net 4.5]
  * **Regions**: change and animate the content of **ContentRegion** (ContentControl) and **ItemsRegions** (ItemsControl, TabControl, ... and more with Adapters) 
  * **ViewModelLocator**: allows to resolve ViewModel for regions and for window with **ResolveWindowViewModel**
  * **RegionManager**: allows to get a region, then **navigate** _with regions_ 
  * **INavigatable**: allows views and view models to be notified on navigate
  * **IActivatable**, **IDeactivatable**: allow to cancel navigation
  * **ILoadedEventListener**: allows to be notified when the view or window is loaded
* **Mvvm**
  * **BootstrapperBase**: bootstrapper base class



* **MvvmLib.Windows** (Navigation) [uwp]
  * **NavigationManager**: allows to create and manage **navigation services**
  * **FrameNavigationService**: allows to **navigate**, go back, go forward, **cancel navigation** and **notify viewmodel**
  * **ViewModelLocator**: allows to **resolve ViewModel** for views
  * **INavigatable**: allows views and view models to be notified on navigate
  * **IDeactivatable**: allows to **cancel** navigation
  * **ILoadedEventListener**: allows to be notified from view model when the **view** is **loaded**
  * **BackRequestManager**: allows to show the **back button** in **title bar**
* **Mvvm**
  * **BootstrapperBase**: bootstrapper base class



* **MvvmLib.Adaptive.Wpf** [net 4.5]
  * **BreakpointBinder**: allows to **bind controls** and make the page "**responsive**"
  * **BreakpointListener**: allows to define a list of breakpoints and be notified on current breakpoint changed



* **MvvmLib.Adaptive.Windows** [uwp]
  * **BreakpointBinder**: allows to **bind controls** and make the page "**responsive**"
  * **BreakpointListener**: allows to define a list of breakpoints and be notified on current breakpoint changed
  * **ResponsiveGridView** and **ResponsiveVariableSizedGridView**
  * **BusyIndicator**



## Installation

### Wpf 

* MvvmLib.Core [required] (BindableBase, Validation, commands, Messenger) [Package NuGet](https://www.nuget.org/packages/MvvmLib.Core/)
* MvvmLib.Wpf [required] (Navigation) [Package NuGet](https://www.nuget.org/packages/MvvmLib.Wpf/)
* MvvmLib.IoC (dependency injection) or use another IoC Container (Unity, StructureMap, etc.) [Package NuGet](https://www.nuget.org/packages/MvvmLib.IoC/)

```
PM> Install-Package MvvmLib.Core
PM> Install-Package MvvmLib.Wpf
PM> Install-Package MvvmLib.IoC
```

* MvvmLib.Adaptive.Wpf [Package NuGet](https://www.nuget.org/packages/MvvmLib.Adaptive.Wpf/)

```
PM> Install-Package MvvmLib.Adaptive.Wpf
```

### Uwp

* MvvmLib.Core [required] (BindableBase, Validation, commands, Messenger) [Package NuGet](https://www.nuget.org/packages/MvvmLib.Core/)
* MvvmLib.Windows [required] (Navigation) [Package NuGet](https://www.nuget.org/packages/MvvmLib.Windows/)
* MvvmLib.IoC (dependency injection) or use another IoC Container (Unity, StructureMap, etc.) [Package NuGet](https://www.nuget.org/packages/MvvmLib.IoC/)

```
PM> Install-Package MvvmLib.Core
PM> Install-Package MvvmLib.Windows
PM> Install-Package MvvmLib.IoC
```

* MvvmLib.Adaptive.Windows [Package NuGet](https://www.nuget.org/packages/MvvmLib.Adaptive.Windows/)

```
PM> Install-Package MvvmLib.Adaptive.Windows
```