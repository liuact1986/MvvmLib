using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    [ContentProperty("Animation")]
    public class ControlledAnimation : DependencyObject
    {
        public AnimationTargetPropertyType? TargetPropertyType
        {
            get { return (AnimationTargetPropertyType?)GetValue(TargetPropertyTypeProperty); }
            set { SetValue(TargetPropertyTypeProperty, value); }
        }

        public static readonly DependencyProperty TargetPropertyTypeProperty =
            DependencyProperty.Register("TargetPropertyType", typeof(AnimationTargetPropertyType?), typeof(ControlledAnimation), new PropertyMetadata(null));

        public string TargetProperty
        {
            get { return (string)GetValue(TargetPropertyProperty); }
            set { SetValue(TargetPropertyProperty, value); }
        }

        public static readonly DependencyProperty TargetPropertyProperty =
            DependencyProperty.Register("TargetProperty", typeof(string), typeof(ControlledAnimation), new PropertyMetadata(null));

        public AnimationTimeline Animation
        {
            get { return (AnimationTimeline)GetValue(AnimationProperty); }
            set { SetValue(AnimationProperty, value); }
        }

        public static readonly DependencyProperty AnimationProperty =
            DependencyProperty.Register("Animation", typeof(AnimationTimeline), typeof(ControlledAnimation), new PropertyMetadata(null));

    }

}
