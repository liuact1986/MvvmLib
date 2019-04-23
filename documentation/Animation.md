## MvvmLib.Animation.Wpf [wpf]

* **AnimatableContentControl** and **animation** classes.

## AnimatableContentControl

Add the namespace to the view

```
xmlns:nav="http://mvvmlib.com/"
```

Add the control and bind dependency properties

```xml
 <nav:AnimatableContentControl nav:RegionManager.ContentRegionName="AnimationSample" 
                                      EntranceAnimation="{Binding EntranceNavAnimation}" 
                                      ExitAnimation="{Binding ExitNavAnimation}"
                                      Simultaneous="{Binding NavSimultaneous}"
                                      Grid.Row="1"/>
```

Create animations and bind to control

```cs
public class MainWindowViewModel : BindableBase
{
    private IContentAnimation entranceNavAnimation;
    public IContentAnimation EntranceNavAnimation
    {
        get { return entranceNavAnimation; }
        set { SetProperty(ref entranceNavAnimation, value); }
    }

    private IContentAnimation exitNavAnimation;
    public IContentAnimation ExitNavAnimation
    {
        get { return exitNavAnimation; }
        set { SetProperty(ref exitNavAnimation, value); }
    }

    private bool navSimultaneous;
    public bool NavSimultaneous
    {
        get { return navSimultaneous; }
        set { SetProperty(ref navSimultaneous , value); }
    }

    public ICommand GoViewACommand { get; }

    public MainWindowViewModel(IRegionNavigationService regionNavigationService)
    {
        var scaleEntranceAnimation = new ScaleAnimation
        {
            From = 0,
            To = 1,
            RenderTransformOrigin = new Point(0.5, 0.5),
            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }
        };
        var scaleExitAnimation = new ScaleAnimation
        {
            From = 1,
            To = 0,
            RenderTransformOrigin = new Point(0.5, 0.5),
            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }
        };

        EntranceNavAnimation = scaleEntranceAnimation;
        ExitNavAnimation = scaleExitAnimation;

        var contentRegion = regionNavigationService.GetContentRegion("AnimationSample");

        GoViewACommand = new RelayCommand(async () =>
        {
            await contentRegion.NavigateAsync(typeof(ViewA));
        });
    }
}
```
 Animation classes awailables:

* **OpacityAnimation**
* **TranslateAnimation**
* **SkewAnimation**
* **ScaleAnimation**
* **RotateAnimation**
* **FxCornerEntranceAnimation** and **FxCornerExitAnimation**
* **FallEntranceAnimation** and **FallExitAnimation**
* **FxVscaleEntranceAnimation** and **FxVscaleExitAnimation**

Create a custom animation class:

```cs
// 1. inherit from ContentAnimationBase or TransformAnimationBase
public class MyAnimation : TransformAnimationBase
{
    // 2. implement, override the base class
    protected override AnimationTimeline[] CreateAnimations()
    {
        // ...
    }

    protected override void SetTargetProperty(AnimationTimeline animation, int index)
    {
        throw new NotImplementedException();
    }
}
```

Example with the OpacityAnimation class:

```cs
public class OpacityAnimation : ContentAnimationBase
{
    protected override double DefaultFrom => 0;
    protected override double DefaultTo => 1;

    protected override void CheckValues()
    {
        if (From < 0 || From > 1)
            throw new ArgumentException("Value between 0 and 1 for an opacity animation");
        if (To < 0 || To > 1)
            throw new ArgumentException("Value between 0 and 1 for an opacity animation");
    }

    protected override void SetTargetProperty(AnimationTimeline animation, int index)
    {
        Storyboard.SetTargetProperty(animation, new PropertyPath("(Control.Opacity)"));
    }
}
```