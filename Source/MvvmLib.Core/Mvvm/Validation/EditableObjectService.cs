using System;
using System.ComponentModel;

namespace MvvmLib.Mvvm
{
    public class EditableObjectService : IEditableObjectService, INotifyPropertyChanged
    {
        private object clonedValue;
        private Type clonedType;

        public Cloner Cloner { get;  }

        public EditableObjectService()
        {
            this.Cloner = new Cloner();
        }

        public void Store(object value)
        {
            if(value == null) { throw new Exception("Value cannot be null"); }

            this.clonedType = value.GetType();
            this.clonedValue = Cloner.DeepClone(clonedType, value);
        }

        public void Restore(object target)
        {
            var properties = this.clonedType.GetProperties();
            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var propertyValue = property.GetValue(this.clonedValue);
                    property.SetValue(target, propertyValue);
                    RaisePropertyChanged(property.Name);
                }
            }
        }

        public void Clear()
        {
            this.clonedValue = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
