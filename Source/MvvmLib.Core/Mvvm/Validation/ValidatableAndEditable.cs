using System;
using System.ComponentModel;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to validate and edit properties.
    /// </summary>
    public class ValidatableAndEditable : Validatable, INotifyDataErrorInfo, IEditableObject
    {
        private readonly IEditableObjectService editableService;

        /// <summary>
        /// Creates the editable.
        /// </summary>
        /// <param name="editableService">The editable object service.</param>
        /// <param name="model">The model / source for validation ("this" if null)</param>
        public ValidatableAndEditable(IEditableObjectService editableService, object model = null)
            : base(model)
        {
            this.editableService = editableService ?? throw new ArgumentNullException(nameof(editableService));
        }

        /// <summary>
        /// Creates the validatatble and editable class.
        /// </summary>
        /// <param name="model">The model / source for validation ("this" if null)</param>
        public ValidatableAndEditable(object model = null)
            : this(new EditableObjectService(), model)
        { }


        /// <summary>
        /// Clones the values.
        /// </summary>
        public void BeginEdit()
        {
            this.editableService.Store(this);
        }

        /// <summary>
        /// Reset the values and errors.
        /// </summary>
        public void CancelEdit()
        {
            this.editableService.Restore(this);
            this.Reset();
            this.EndEdit();
        }

        /// <summary>
        /// Clear the cloned values and notify changes.
        /// </summary>
        public void EndEdit()
        {
            this.editableService.Clear();
            this.RaisePropertyChanged(string.Empty);
        }

    }
}
