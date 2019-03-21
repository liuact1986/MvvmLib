# MvvmLib

>  Mvvm Library for .NET projects.

This package is going to merge my 3 other projects ([WpfLib](https://github.com/romagny13/WpfLib), [UwpLib](https://github.com/romagny13/UwpLib), [XFLib](https://github.com/romagny13/XFLib)) in one single solution. 

## MvvmLib.Core [netstandard 2.0, net 4.5]

* **BindableBase**, **Validatable**, **Editable**, **ValidateAndEditable** and **ModelWrapper** base classes for _Models and ViewModels_
* **Commands** and **composite** command
* **Sync** _extensions for list and collections_. Allows to **sync data**
* **Singleton**
* **Messenger** : allows to **subscribe**, **publish** and filter messages


## MvvmLib.IoC (Dependency Injection) [netstandard 2.0, net 4.5]
  * **Injector**: allows to **register Types, Singletons, Instances, values, auto discover types, etc.**.
  


## MvvmLib.Wpf (Navigation) [net 4.5]

* **Regions**: change and animate the content of **ContentRegion** (ContentControl) and **ItemsRegions** (ItemsControl, TabControl, ... and more with Adapters) 
* **ViewModelLocator**: allows to resolve ViewModel for regions and for window with **ResolveWindowViewModel**
* **RegionManager**: allows to get a region, then **navigate** _with regions_ 
* **INavigatable**: allows views and view models to be notified on navigate
* **IActivatable**, **IDeactivatable**: allow to cancel navigation
* **ILoadedEventListener**: allows to be notified when the view or window is loaded
* **BootstrapperBase**: bootstrapper base class


### MvvmLib.Windows (Navigation) [uwp]
  
* **NavigationManager**: allows to create and manage **navigation services**
* **FrameNavigationService**: allows to **navigate**, go back, go forward, **cancel navigation** and **notify viewmodel**
* **ViewModelLocator**: allows to **resolve ViewModel** for views
* **INavigatable**: allows views and view models to be notified on navigate
* **IDeactivatable**: allows to **cancel** navigation
* **ILoadedEventListener**: allows to be notified from view model when the **view** is **loaded**
* **BackRequestManager**: allows to show the **back button** in **title bar**
* **BootstrapperBase**: bootstrapper base class

## MvvmLib.Adaptive.Wpf [net 4.5]

* **BreakpointBinder**: allows to **bind controls** and make the page "**responsive**"
* **BreakpointListener**: allows to define a list of breakpoints and be notified on current breakpoint changed

## MvvmLib.Adaptive.Windows [uwp]

* **BreakpointBinder**: allows to **bind controls** and make the page "**responsive**"
* **BreakpointListener**: allows to define a list of breakpoints and be notified on current breakpoint changed
* **ResponsiveGridView** and **ResponsiveVariableSizedGridView**
* **BusyIndicator**


## Installation

### Wpf 

| Package  | Required | Description | NuGet |
| --- | --- | --- | --- |
| [MvvmLib.Core](https://www.nuget.org/packages/MvvmLib.Core/)  | Yes  | BindableBase, Validation, commands, Messenger | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.Core.svg?style=flat-square)
| [MvvmLib.Wpf](https://www.nuget.org/packages/MvvmLib.Wpf/) | Yes  | Navigation with view model, view composition | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.Wpf.svg?style=flat-square) |
| [MvvmLib.IoC](https://www.nuget.org/packages/MvvmLib.IoC/) | No  | IoC Container or use Unity, Autofac, etc. | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.IoC.svg?style=flat-square) |
| [MvvmLib.Adaptive.Wpf](https://www.nuget.org/packages/MvvmLib.Adaptive.Wpf/) | No | Make the view "responsive" | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.Adaptive.Wpf.svg?style=flat-square) |

```
PM> Install-Package MvvmLib.Core
PM> Install-Package MvvmLib.Wpf
PM> Install-Package MvvmLib.IoC
PM> Install-Package MvvmLib.Adaptive.Wpf
```

### Uwp

| Package  | Required | Description | NuGet |
| --- | --- | --- | --- |
| [MvvmLib.Core](https://www.nuget.org/packages/MvvmLib.Core/)  | Yes | BindableBase, Validation, commands, Messenger | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.Core.svg?style=flat-square) |
| [MvvmLib.Windows](https://www.nuget.org/packages/MvvmLib.Windows/) | Yes | Navigation with view model | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.Windows.svg?style=flat-square) |
| [MvvmLib.IoC](https://www.nuget.org/packages/MvvmLib.IoC/) | No | IoC Container or use Unity, Autofac, etc. | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.IoC.svg?style=flat-square) |
| [MvvmLib.Adaptive.Windows](https://www.nuget.org/packages/MvvmLib.Adaptive.Windows/) | No | Make the page "responsive" | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.Adaptive.Windows.svg?style=flat-square) |

```
PM> Install-Package MvvmLib.Core
PM> Install-Package MvvmLib.Windows
PM> Install-Package MvvmLib.IoC
PM> Install-Package MvvmLib.Adaptive.Windows
```
