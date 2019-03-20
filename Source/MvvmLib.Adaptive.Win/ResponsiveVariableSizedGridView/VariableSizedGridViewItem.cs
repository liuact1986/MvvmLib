using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MvvmLib.Adaptive
{
    public class VariableSizedGridViewItem : INotifyPropertyChanged
    {
        private int _columnSpan;
        public int ColumnSpan
        {
            get { return _columnSpan; }
            set { SetProperty(ref _columnSpan, value); }
        }

        private int _rowSpan;
        public int RowSpan
        {
            get { return _rowSpan; }
            set { SetProperty(ref _rowSpan, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
