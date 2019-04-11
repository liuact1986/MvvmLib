using System;
using Xamarin.Forms;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// ALlows to discover and bind the view model to  the view.
    /// </summary>
    public class ViewModelLocator
    {

        public static readonly BindableProperty ResolveViewModelProperty =
           BindableProperty.CreateAttached("ResolveViewModel", typeof(bool?), typeof(ViewModelLocator), null,
               propertyChanged: OnResolveViewModelChanged);

        public static bool? GetResolveViewModel(BindableObject bindable)
        {
            return (bool?)bindable.GetValue(ViewModelLocator.ResolveViewModelProperty);
        }

        public static void SetResolveViewModel(BindableObject bindable, bool? value)
        {
            bindable.SetValue(ViewModelLocator.ResolveViewModelProperty, value);
        }

        private static void OnResolveViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (((bool?)newValue) == true)
            {
                var view = bindable as BindableObject;
                if (view != null)
                {
                    Type viewModelType = ViewModelLocationProvider.ResolveViewModelType(view.GetType());
                    object viewModel = null;
                    if (viewModelType != null)
                    {
                        viewModel = ViewModelLocationProvider.ResolveViewModel(viewModelType);
                    }
                    view.BindingContext = viewModel;
                }
            }
        }

        //public static object GetViewModel(Type viewType)
        //{
        //    var viewModelType = ViewModelLocationProvider.ResolveViewModelType(viewType);
        //    if (viewModelType != null)
        //    {
        //        return ViewModelLocationProvider.ResolveViewModel(viewModelType);
        //    }
        //    return null;
        //}

        //public static bool IsNullDataContext(BindableObject view)
        //{
        //    return view.BindingContext == null;
        //}

        //public static void SetViewModel(BindableObject view, object viewModel)
        //{
        //    if (IsNullDataContext(view))
        //    {
        //        view.BindingContext = viewModel;
        //    }
        //}
    }

}
