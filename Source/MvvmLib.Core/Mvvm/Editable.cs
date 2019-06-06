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
        protected ObjectEditor editor;


        /// <summary>
        /// Creates the editable.
        /// </summary>
        public Editable()
        {
            this.editor = new ObjectEditor(this.GetType());
        }

        /// <summary>
        /// Clones the values.
        /// </summary>
        public void BeginEdit()
        {
            this.editor.Store(this);
        }

        /// <summary>
        /// Reset the values.
        /// </summary>
        public void CancelEdit()
        {
            this.editor.Restore();
            this.OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Clear the cloned values and notify changes.
        /// </summary>
        public void EndEdit()
        {
            this.editor.Clean();
        }
    }
}
