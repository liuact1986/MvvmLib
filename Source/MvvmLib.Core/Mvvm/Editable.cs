using System.ComponentModel;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Implements <see cref="IEditableObject" />. Allows to cancel changes to an object.
    /// </summary>
    public class Editable : BindableBase, IEditableObject
    {
        private readonly ObjectEditor objectEditor;
        /// <summary>
        /// The object editor.
        /// </summary>
        public ObjectEditor ObjectEditor
        {
            get { return objectEditor; }
        }

        /// <summary>
        /// Creates the editable.
        /// </summary>
        public Editable()
        {
            this.objectEditor = new ObjectEditor();
        }

        /// <summary>
        /// Clones the values.
        /// </summary>
        public void BeginEdit()
        {
            this.objectEditor.Store(this);
        }

        /// <summary>
        /// Reset the values.
        /// </summary>
        public void CancelEdit()
        {
            this.objectEditor.Restore();
            this.OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Clear the cloned values and notify changes.
        /// </summary>
        public void EndEdit()
        {
            this.objectEditor.Clean();
        }
    }
}
