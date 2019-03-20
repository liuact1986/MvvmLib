using System.ComponentModel;

namespace MvvmLib.Mvvm
{
    public class ValidatableAndEditable : Validatable, IEditableObject
    {
        protected IEditableObjectService editableService;

        public ValidatableAndEditable()
            : this(new EditableObjectService())
        { }

        public ValidatableAndEditable(IEditableObjectService editableService)
        {
            this.editableService = editableService;
        }

        public void BeginEdit()
        {
            this.editableService.Store(this);
        }

        public void CancelEdit()
        {
            this.editableService.Restore(this);
        }

        public void EndEdit()
        {
            this.editableService.Clear();
            this.RaisePropertyChanged(string.Empty);
        }
    }

    //public class ValidatableAndEditable : Validatable, IEditableObject
    //{
    //    protected IEditableObjectService editableService;

    //    public ValidatableAndEditable()
    //        : this(new EditableObjectService())
    //    { }

    //    public ValidatableAndEditable(IEditableObjectService editableService)
    //    {
    //        this.editableService = editableService;
    //    }

    //    public void BeginEdit()
    //    {
    //        this.editableService.Store(this);
    //    }

    //    public void CancelEdit()
    //    {
    //        this.editableService.Restore(this);
    //    }

    //    public void EndEdit()
    //    {
    //        this.editableService.Clear();
    //        this.RaisePropertyChanged(string.Empty);
    //    }
    //}
}
