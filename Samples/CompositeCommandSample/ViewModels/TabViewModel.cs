using CompositeCommandSample.Common;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System;
using System.ComponentModel;
using System.Windows;

namespace CompositeCommandSample.ViewModels
{
    // isolate model for Tracking changes (avoid infinite loop on cloning)
    //public class MyItem : BindableBase
    //{
    //    private string title;
    //    public string Title
    //    {
    //        get { return title; }
    //        set { SetProperty(ref title, value); }
    //    }
    //}

    public class TabViewModel : BindableBase //, INavigatable
    {
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (SetProperty(ref title, value) && Tracker != null)
                    Tracker.CheckChanges();
            }
        }

        //public MyItem Item { get; set; }

        private string saveMessage;
        public string SaveMessage
        {
            get { return saveMessage; }
            set { SetProperty(ref saveMessage, value); }
        }

        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set
            {
                SetProperty(ref canSave, value);
                //if (SetProperty(ref canSave, value))
                //{
                //    SaveCommand.RaiseCanExecuteChanged(); // or with ObserveProperty
                //}
            }
        }

        public IRelayCommand SaveCommand { get; set; }
        public ChangeTracker Tracker { get; set; }

        public TabViewModel()
        {

        }

        public TabViewModel(IApplicationCommands applicationCommands, string title)
        {
            canSave = true;
            SaveCommand = new RelayCommand(OnSave, CheckCanSave)
                .ObserveProperty(() => CanSave);

            applicationCommands.SaveAllCommand.Add(SaveCommand);

            this.Title = title;
            this.Tracker = new ChangeTracker(this);
            // this.Item = new MyItem { Title = title };
            //this.Tracker = new ChangeTracker(this.Item);

            // this.Item.PropertyChanged += OnItemPropertyChanged;
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Tracker.CheckChanges();
        }

        private void OnSave()
        {
            var message = $"Save TabView {Title}! {DateTime.Now.ToLongTimeString()}";
            MessageBox.Show(message);
            SaveMessage = message;
        }

        private bool CheckCanSave()
        {
            return canSave;
        }
    }

}
