using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    /// <summary>
    /// Animation used by the <see cref="TransitioningItemsControl"/>. Avoid to set the target of the Storyboard and provides shortcut for the target proterty.
    /// </summary>
    [ContentProperty("Animation")]
    public class ControlledAnimation : DependencyObject
    {
        /// <summary>
        /// The target property Type is a shortcut for the TragetProperty. 
        /// </summary>
        public AnimationTargetPropertyType? TargetPropertyType
        {
            get { return (AnimationTargetPropertyType?)GetValue(TargetPropertyTypeProperty); }
            set { SetValue(TargetPropertyTypeProperty, value); }
        }

        /// <summary>
        /// The target property Type is a shortcut for the TragetProperty. 
        /// </summary>
        public static readonly DependencyProperty TargetPropertyTypeProperty =
            DependencyProperty.Register("TargetPropertyType", typeof(AnimationTargetPropertyType?), typeof(ControlledAnimation), new PropertyMetadata(null));

        /// <summary>
        /// The target property.
        /// </summary>
        public string TargetProperty
        {
            get { return (string)GetValue(TargetPropertyProperty); }
            set { SetValue(TargetPropertyProperty, value); }
        }

        /// <summary>
        /// The target property.
        /// </summary>
        public static readonly DependencyProperty TargetPropertyProperty =
            DependencyProperty.Register("TargetProperty", typeof(string), typeof(ControlledAnimation), new PropertyMetadata(null));

        /// <summary>
        /// The animation.
        /// </summary>
        public AnimationTimeline Animation
        {
            get { return (AnimationTimeline)GetValue(AnimationProperty); }
            set { SetValue(AnimationProperty, value); }
        }

        /// <summary>
        /// The animation.
        /// </summary>
        public static readonly DependencyProperty AnimationProperty =
            DependencyProperty.Register("Animation", typeof(AnimationTimeline), typeof(ControlledAnimation), new PropertyMetadata(null));

    }

}
