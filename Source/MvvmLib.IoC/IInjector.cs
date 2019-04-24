namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to register or discover types, factories, instances then create instances and inject dependencies.
    /// </summary>
    public interface IInjector : IInjectorRegistry, IInjectorResolver
    {

    }

}