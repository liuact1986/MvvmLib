using System.ComponentModel;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Implements <see cref="IEditableObject" />. Allows to cancel changes to an object.
    /// </summary>
    public class Editable : BindableBase, IEditableObject
    {
        /// <summary>
        /// The editable object service.
        /// </summary>
        protected IEditableObjectService editableService;

        /// <summary>
        /// Creates the editable.
        /// </summary>
        public Editable()
            : this(new EditableObjectService())
        { }

        /// <summary>
        /// Creates the editable.
        /// </summary>
        /// <param name="editableService">The editable object service.</param>
        public Editable(IEditableObjectService editableService)
        {
            this.editableService = editableService ?? throw new System.ArgumentNullException(nameof(editableService));
        }

        /// <summary>
        /// Clones the values.
        /// </summary>
        public void BeginEdit()
        {
            this.editableService.Store(this);
        }

        /// <summary>
        /// Reset the values.
        /// </summary>
        public void CancelEdit()
        {
            this.editableService.Restore(this);
            this.OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Clear the cloned values and notify changes.
        /// </summary>
        public void EndEdit()
        {
            this.editableService.Reset();
        }
    }
}
