﻿using System;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class ScaleAnimation : TransformAnimationBase
    {
        protected override double DefaultFrom => 1;
        protected override double DefaultTo => 1;
        private readonly TransformDirection defaultTransformDirection = TransformDirection.X;
        protected virtual TransformDirection DefaultTransformDirection => defaultTransformDirection;

        private ScaleTransform scale;
        public ScaleTransform Scale
        {
            get { return scale; }
            protected set { scale = value; }
        }

        protected TransformDirection? transformDirection;
        public TransformDirection TransformDirection
        {
            get { return transformDirection ?? DefaultTransformDirection; }
            set { transformDirection = value; }
        }

        private double? scaleX;
        public double? ScaleX
        {
            get { return scaleX; }
            set { scaleX = value; }
        }

        private double? scaleY;
        public double? ScaleY
        {
            get { return scaleY; }
            set { scaleY = value; }
        }

        private double? centerX;
        public double? CenterX
        {
            get { return centerX; }
            set { centerX = value; }
        }

        private double? centerY;
        public double? CenterY
        {
            get { return centerY; }
            set { centerY = value; }
        }

        protected override AnimationTimeline CreateAnimation()
        {
            if (From < -1 || From > 1) { throw new ArgumentException("Value between 0 and 1 for a scale animation"); }
            if (To < -1 || To > 1) { throw new ArgumentException("Value between 0 and 1 for a scale animation"); }

            Scale = new ScaleTransform();
            if (ScaleX.HasValue)
                Scale.ScaleX = ScaleX.Value;
            if (ScaleY.HasValue)
                Scale.ScaleY = ScaleY.Value;
            if (CenterX.HasValue)
                Scale.CenterX = CenterX.Value;
            if (CenterY.HasValue)
                Scale.CenterY = CenterY.Value;

            Element.RenderTransform = Scale;
            Element.RenderTransformOrigin = RenderTransformOrigin;

            var animation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                animation.EasingFunction = EasingFunction;

            return animation;
        }

        protected override void BeginAnimation()
        {
            var dp = TransformDirection == TransformDirection.X ?
                ScaleTransform.ScaleXProperty
                : ScaleTransform.ScaleYProperty;

            Scale.BeginAnimation(dp, Animation);
        }

        public override void CancelAnimation()
        {
            if (Element != null)
            {
                var dp = TransformDirection == TransformDirection.X ?
                   ScaleTransform.ScaleXProperty
                   : ScaleTransform.ScaleYProperty;

                Scale.BeginAnimation(dp, null);
                AnimationWasCancelled = true;
                IsAnimating = false;
            }
        }
    }


}