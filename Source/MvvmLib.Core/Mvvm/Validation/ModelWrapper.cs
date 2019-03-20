using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{

    public class ModelWrapper<TModel> : Validatable , IEditableObject
    {
        public TModel Model { get; set; }

        protected IEditableObjectService editableObjectService;

        public ModelWrapper(TModel model, IEditableObjectService editableObjectService)
            : base(model)
        {
            Model = model;
            this.editableObjectService = editableObjectService;
        }

        public ModelWrapper(TModel model)
           : this(model, new EditableObjectService())
        { }

        protected virtual TValue GetValue<TValue>([CallerMemberName]string propertyName = null)
        {
            return (TValue)typeof(TModel).GetProperty(propertyName).GetValue(Model);
        }

        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName]string propertyName = null)
        {
            typeof(TModel).GetProperty(propertyName).SetValue(Model, value);
            RaisePropertyChanged(propertyName);
            RaisePropertyChanged(string.Empty);
            if (ValidationType == ValidationType.OnPropertyChange)
            {
                this.ValidateProperty(propertyName, value);
            }
        }

        #region Editable 

        public void BeginEdit()
        {
            this.editableObjectService.Store(Model);
        }

        public void CancelEdit()
        {
            this.editableObjectService.Restore(Model);
            this.RaisePropertyChanged(string.Empty);
        }

        public void EndEdit()
        {
            this.editableObjectService.Clear();
            this.RaisePropertyChanged(string.Empty);
        }

        #endregion // Editable
    }


}
