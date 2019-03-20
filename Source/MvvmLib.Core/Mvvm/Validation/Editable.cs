using System.ComponentModel;

namespace MvvmLib.Mvvm
{
    public class Editable : BindableBase, IEditableObject
    {
        protected IEditableObjectService editableService;

        public Editable()
            : this(new EditableObjectService())
        { }

        public Editable(IEditableObjectService editableService)
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
            this.EndEdit();
        }

        public void EndEdit()
        {
            this.editableService.Clear();
            this.RaisePropertyChanged(string.Empty);
        }
    }
}
