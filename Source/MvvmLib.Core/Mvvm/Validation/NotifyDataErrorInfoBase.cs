using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace MvvmLib.Mvvm
{
    public class NotifyDataErrorInfoBase : BindableBase, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> errorsByPropertyName
            = new Dictionary<string, List<string>>();

        public bool HasErrors => errorsByPropertyName.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool ContainErrors(string propertyName)
        {
            return errorsByPropertyName.ContainsKey(propertyName);
        }

        public bool ContainError(string propertyName, string error)
        {
            return ContainErrors(propertyName)
                && errorsByPropertyName[propertyName].Contains(error);
        }

        public void AddError(string propertyName, string error)
        {
            if (!ContainErrors(propertyName))
            {
                errorsByPropertyName[propertyName] = new List<string>
                {
                    error
                };
                this.RaiseErrorsChanged(propertyName);
            }
            else if (!ContainError(propertyName, error))
            {
                errorsByPropertyName[propertyName].Add(error);
                this.RaiseErrorsChanged(propertyName);
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (ContainErrors(propertyName))
            {
                return errorsByPropertyName[propertyName];
            }
            else
            {
                return null;
            }
        }

        public void ClearErrors(string propertyName)
        {
            if (ContainErrors(propertyName))
            {
                errorsByPropertyName.Remove(propertyName);
                this.RaiseErrorsChanged(propertyName);
            }
        }

        protected virtual void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            base.RaisePropertyChanged(nameof(HasErrors));
        }
    }


}
