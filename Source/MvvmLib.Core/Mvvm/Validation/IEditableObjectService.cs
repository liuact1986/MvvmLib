namespace MvvmLib.Mvvm
{
    public interface IEditableObjectService
    {
        Cloner Cloner { get; }

        void Clear();
        void Restore(object target);
        void Store(object value);
    }

}
