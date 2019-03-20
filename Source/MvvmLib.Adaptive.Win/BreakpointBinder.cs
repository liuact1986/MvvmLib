using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MvvmLib.Adaptive
{
    public class DeferredActive
    {
        public double Width { get; set; }
        public Dictionary<string, object> Bindings { get; set; }

        public DeferredActive(double width, Dictionary<string, object> bindings)
        {
            this.Width = width;
            this.Bindings = bindings;
        }
    }
    /// <summary>
    /// Allows to bind by DataContext / ViewModel
    /// </summary>
    public class BreakpointBinder : Control, INotifyPropertyChanged
    {
        public static readonly DependencyProperty FileProperty =
            DependencyProperty.Register("File", typeof(string), typeof(BreakpointBinder), new PropertyMetadata(null, OnFileChanged));

        public string File
        {
            get { return (string)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        private static async void OnFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignMode.IsInDesignModeStatic)
            {
                var control = (BreakpointBinder)d;
                await control.LoadFromJsonFileAsync(e.NewValue.ToString());
            }
        }

        protected BreakpointListener breakpointListener;
        protected AdaptiveJsonFileService jsonService;

        protected DeferredActive deferred;

        protected readonly List<EventHandler<ActiveChangedEventArgs>> activeChanged;
        public event EventHandler<ActiveChangedEventArgs> ActiveChanged
        {
            add
            {
                if (!activeChanged.Contains(value))
                {
                    activeChanged.Add(value);
                }
                if(deferred != null)
                {
                    this.RaiseActiveChanged(deferred.Width, deferred.Bindings);
                }
            }
            remove { if (activeChanged.Contains(value)) activeChanged.Remove(value); }
        }

        public SortedList<double, Dictionary<string, object>> BindingsByWidth { get; }

        private Dictionary<string, object> active;
        public Dictionary<string, object> Active
        {
            get { return active; }
            set
            {
                this.active = value;
                this.RaisePropertyChanged("Active");
            }
        }

        public BreakpointBinder() : this(new MainWindowSizeChangeStrategy())
        { }

        public BreakpointBinder(IAdaptiveSizeChangeStrategy sizeChangeStrategy)
        {
            this.Visibility = Visibility.Collapsed;
            this.breakpointListener = new BreakpointListener(sizeChangeStrategy);
            this.jsonService = new AdaptiveJsonFileService();
            this.activeChanged = new List<EventHandler<ActiveChangedEventArgs>>();
            this.BindingsByWidth = new SortedList<double, Dictionary<string, object>>();
            this.breakpointListener.BreakpointChanged += OnBreakpointChanged;
        }

        public BreakpointBinder AddBreakpointWithBindings(double width, Dictionary<string, object> bindings)
        {
            if (!this.BindingsByWidth.ContainsKey(width))
            {
                this.BindingsByWidth[width] = bindings;
                this.breakpointListener.AddBreakpoint(width);
            }
            return this;
        }

        public async Task LoadFromJsonFileAsync(string path)
        {
            var jsonSettings = await jsonService.LoadAsync(path);
            foreach (var jsonSetting in jsonSettings)
            {
                this.AddBreakpointWithBindings(jsonSetting.minwidth, jsonSetting.bindings);
            }
        }

        private void OnBreakpointChanged(object sender, BreakpointChangedEventArgs e)
        {
            var width = e.Width;
            var bindings = this.TryGetActive(width);
            if (bindings != null)
            {
                this.Active = bindings;
                this.RaiseActiveChanged(width, bindings);
            }
        }

        public Dictionary<string, object> TryGetActive(double width)
        {
            Dictionary<string, object> bindings = null;
            foreach (var entry in this.BindingsByWidth)
            {
                if (entry.Key <= width)
                {
                    bindings = entry.Value;
                }
            }
            return bindings;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaiseActiveChanged(double width, Dictionary<string, object> bindings)
        {
            var context = new ActiveChangedEventArgs(width, bindings);
            if (this.activeChanged.Count > 0)
            {
                foreach (var handler in activeChanged)
                {
                    handler(this, context);
                }
                this.deferred = null;
            }
            else
            {
                this.deferred = new DeferredActive(width, bindings);
            }
        }
    }

}
