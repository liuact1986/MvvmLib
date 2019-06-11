# MvvmLib

>  Mvvm Library for .NET projects.

This package is going to merge my 3 other projects ([WpfLib](https://github.com/romagny13/WpfLib), [UwpLib](https://github.com/romagny13/UwpLib), [XFLib](https://github.com/romagny13/XFLib)) in one single solution. 

## MvvmLib.Core [netstandard 2.0, net 4.5]

* **BindableBase**, **Editable**, **Validatable** and **ModelWrapper** base classes for _Models and ViewModels_
* **ChangeTracker**: allows to track object changes.
* **NotifyPropertyChangedObserver** and **FilterableNotifyPropertyChangedObserver**: allows to observe and filter an object that implements INotifyPropertyChanged
* **Commands** and **composite** command
* **SyncUtils** Allows to **sync lists and collections** 
* **Singleton**
* **EventAggregator** : allows to **subscribe**, **publish** and filter messages


## MvvmLib.IoC (Dependency Injection) [netstandard 2.0, net 4.5]

* Allows to **register** and **resolve** Type, Singleton, Instance, factory, values
* **Auto discover** types and types for interfaces not registered
* **Inject** properties and manage circular references
* Attributes: **PreferredConstructor**, **PreferredImplementation** (for interfaces), **Dependency** (for properties) 

## MvvmLib.Wpf (Navigation) [net 4.5]

* **NavigationSource**: navigation for _ContentControl_
* **SharedSource**: for _ItemsControl_, _Selector_, etc.
* **AnimatableContentControl**, **TransitioningContentControl**, **TransitioningItemsControl**: allow to animate content
* **NavigationManager**: allows to manage NavigationSources and SharedSources
* **NavigationBrowser**: allows to browse items sources.
* **INavigationAware**: allows _view models_ to be notified on navigate
* **ICanActivate**, **ICanDeactivate**: allow to cancel navigation
* **IIsSelected**, **ISelectable**, **SelectionSyncBehavior**: allow to select a view 
* **Navigation Behaviors**: **SelectionSyncBehavior** and **EventToCommandBehavior**
* **ViewModelLocator**: allows to **resolve ViewModel** for **views**
* **IIsLoaded**: allows to notify view model that the view is loaded for a view that use resolve view model attached property.
* **BootstrapperBase**: base class for Bootstrapper

## MvvmLib.Windows (Navigation) [uwp]
  
* **NavigationManager**: allows to create and manage **navigation services**
* **FrameNavigationService**: allows to **navigate**, go back, go forward, **cancel navigation** and **notify viewmodel**
* **ViewModelLocator**: allows to **resolve ViewModel** for views
* **INavigatable**: allows the view models to be notified on navigate
* **IDeactivatable**: allows to **cancel** navigation
* **ILoadedEventListener**: allows to be notified from view model when the **view** is **loaded**
* **BackRequestManager**: allows to show the **back button** in **title bar**
* **BootstrapperBase**: bootstrapper base class

## MvvmLib.XF (Navigation) [Xamarin]
  
* **NavigationManager**: allows to create and manage **navigation services**
* **PageNavigationService**: allows to **push**, push modal, pop, pop modal, pop to root, handle system go back, **cancel navigation** and **notify viewmodel**
* **ViewModelLocator**: allows to **resolve ViewModel** for views
* **INavigatable**: allows the view models to be notified on navigate
* **IActivatable**, **IDeactivatable**: allow to cancel navigation
* **INavigationParameterKnowledge**: allows to store navigation parameter in the view model for system go back
* **IPageKnowledge**: allows to receive in view model the page
* **DialogService**: allows to display alerts and action sheets
* **BootstrapperBase**: bootstrapper base class
* **EventToCommandBehavior** and **BehaviorBase**

## MvvmLib.Animation.Wpf [wpf]

* **AnimatableContentControl** and **animation** classes.

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

### Xamarin

| Package  | Required | Description | NuGet |
| --- | --- | --- | --- |
| [MvvmLib.Core](https://www.nuget.org/packages/MvvmLib.Core/)  | No | BindableBase, Validation, commands, Messenger | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.Core.svg?style=flat-square) |
| [MvvmLib.XF](https://www.nuget.org/packages/MvvmLib.XF/) | Yes | Navigation with view model | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.XF.svg?style=flat-square) |
| [MvvmLib.IoC](https://www.nuget.org/packages/MvvmLib.IoC/) | No | IoC Container or use Unity, Autofac, etc. | ![Nuget](https://img.shields.io/nuget/v/MvvmLib.IoC.svg?style=flat-square) |

```
PM> Install-Package MvvmLib.Core
PM> Install-Package MvvmLib.XF
PM> Install-Package MvvmLib.IoC
```

<p align="center">
<img src="https://res.cloudinary.com/romagny13/image/upload/v1553188957/mvvm_logo_xxv5gn.png">
</p>
