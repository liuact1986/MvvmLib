# DI \(Dependency Injection\) with Injector

The injector can **resolve all types automatically** (views, view models, ect.) 

**Require to be registered :**

* All **types with** their **interfaces**
* **Singletons**


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

By default, the injector try to resolve automatically non registered types.

To change this behavior:

```cs
injector.AutoDiscovery = false;
```

## NonPublicConstructors and NonPublicProperties

By default, the injector find public and private constructors/ properties

To change this behavior:

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

By default, the injector use the ExpressionDelegateFactoryType (create instances with expressions linq)

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