# DI \(Dependency Injection\) with Injector

> The injector allows to **resolve all types and types for implemented interfaces automatically** (views, view models, services) 

  * Allows to **register** and **resolve** Type, Singleton, Instance, factory, values
  * **Auto discover** types and types for interfaces not registered
  * **Inject** properties and manage circular references
  * Attributes: **PreferredConstructor**, **PreferredImplementation** (for interfaces), **Dependency** (for properties) 


**Require to be registered :**

* **Singletons**
* Use the **PreferredImplementation attribute** with many implementation of interfaces.


Namespace: required for extensions methods

```cs
using MvvmLib.Ioc;
```

 Inject the service (recommanded)

```cs
public class MyViewModel
{
    public MyViewModel(IInjector injector)
    {

    }
}
```

## Registering

### Constructor Injection

Types

```cs
injector.RegisterType<Item>();
injector.RegisterType<IMyService, MyService>();
// with key
injector.RegisterType<Item>("my key");

// with value container (for value types, nullables, Uri, enumerables)...
injector.RegisterType<Item>().WithValueContainer(new ValueContainer { { "myString", "my value" } });
```

Singleton

```cs
injector.RegisterSingleton<Item>();
// with key
injector.RegisterSingleton<Item>("my key");
```

Instance

```cs
injector.RegisterInstance<Item>(new Item());
// with key
injector.RegisterInstance<Item>("my key", new Item());
```

Factory

```cs
injector.RegisterFactory<Item>(() => new Item());
// with key
injector.RegisterFactory<Item>("my key", () => new Item());
```

Func

```cs
public class WithFunc
{
    public Item MyItem { get; set; }

    public WithFunc(Func<Item> myFunc)
    {
        MyItem = myFunc(); 
    }
}
```

```cs
injector.RegisterInstance<Item>(new Item { MyString = "My value" });
injector.RegisterType<WithFunc>();

var instance = service.GetInstance<WithFunc>(); 
```

The injector resolves not registered types for **interfaces**. For many implementations use the **PreferredImplementationAttribute** Example:

```cs
public interface IMyService
{ }

public class MyService1 : IMyService
{ }

[PreferredImplementation]
public class MyService2 : IMyService
{ }

public class MyService3 : IMyService
{ }
```

Its possible to register services (interfaces + implemenation) discovered as singletons:

```cs
injector.LifetimeOnDiscovery = LifetimeOnDiscovery.SingletonOnlyForServices;
```

### Registering Type with interfaces

```cs
public interface ILookupDataServiceType1 { }

public interface ILookupDataServiceType2 { }

public interface ILookupDataServiceType3 { }

public class LookupDataService : ILookupDataServiceType1, ILookupDataServiceType2, ILookupDataServiceType3 
{ }
```

```cs
injector.RegisterTypeWithInterfaces<LookupDataService>();
```

Avoid to do:

```cs
injector.RegisterTypeWithInterfaces<ILookupDataServiceType1, LookupDataService>();
injector.RegisterTypeWithInterfaces<ILookupDataServiceType2, LookupDataService>();
injector.RegisterTypeWithInterfaces<ILookupDataServiceType3, LookupDataService>();
```

Set a registration as singleton

```cs
var registrationOptionsContainer = injector.RegisterTypeWithInterfaces<LookupDataService>();
var registrationOptions2 = container[typeof(ILookupDataServiceType2)];
registrationOptions2.AsSingleton();
```

### Registering Singleton with interfaces

Allows to get the same instance for all "services".

```cs
injector.RegisterSingletonWithInterfaces<LookupDataService>();
```

Set a registration as multi instances

```cs
var registrationOptionsContainer = injector.RegisterSingletonWithInterfaces<LookupDataService>();
var registrationOptions2 = container[typeof(ILookupDataServiceType2)];
registrationOptions2.AsMultiInstances();
```

### Property Injection

With Dependency attribute

```cs
public class SubItem
{
    // properties, fields, ...
}

public class Item
{
    [Dependency] 
    // or with name 
    // [Dependency(Name = "my key")]
    public SubItem MySubItem { get; set; }

    [Dependency] 
    public string MyString { get; set; }
}
```

```cs
injector.RegisterInstance<MySubItem>(new SubItem());
injector.RegisterType<Item>().WithValueContainer(new ValueContainer { { "myString", "my value" } });

var instance = injector.BuildUp<Item>();
```

Or AutoDiscovery

```cs
var instance = injector.BuildUp<Item>();
```


With OnResolved (allows to handle circular references)


```cs
public class CircularPropertyItem
{
    public CircularPropertyItem Item { get; set; } // circular reference

    // other properties
    public string MyString { get; set; }
    public SubItem MySubItem { get; set; }
}
```

```cs
injector.RegisterType<CircularPropertyItem>().OnResolved((registration, instance) =>
{
    var item = instance as CircularPropertyItem;
    item.Item = item; // circular reference

    // other properties
    item.MyString = "My value";
    item.MySubItem = injector.GetInstance<SubItem>();
});

var result = injector.GetInstance<CircularPropertyItem>();
```

## GetInstance

```cs
var item = injector.GetInstance<Item>(); 
var service = injector.GetInstance<IMyService>();
// with key
var item2 = injector.GetInstance<Item>("my key");
```

Get new Instance (only for Types)

```cs
var item = injector.GetNewInstance<Item>();
```

Get all instances of Type

```cs
var instances = service.GetAllInstances<Item>();
```

## AutoDiscovery

By default, the non registered types are resolved. To change this behavior:

```cs
injector.AutoDiscovery = false;
```

## NonPublicConstructors and NonPublicProperties

By default, non public constructors/ properties are found. To change this behavior:

```cs
injector.NonPublicConstructors = false;
injector.NonPublicProperties = false;
```

## PreferredConstructorAttribute

Example

```cs
public class MyViewModel
{
    public MyViewModel()
    {

    }

    public MyViewModel(IRegionManager regionManager)
    {

    }

    [PreferredConstructor]
    public MyViewModel(IRegionManager regionManager, IMessenger messenger)
    {

    }
}
```

## DelegateFactoryType

The **ExpressionDelegateFactoryType** is used by **default** to create instances (Expressions **Linq**).

Change to ReflectionDelegateFactory (less performant)

```cs
injector.DelegateFactoryType = DelegateFactoryType.Reflection;
```

## Events

* Registered
* Resolved

```cs
injector.Registered += (s, e) => { };

injector.Resolved += (s, e) => {  };
```