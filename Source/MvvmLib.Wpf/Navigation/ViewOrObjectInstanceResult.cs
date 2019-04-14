namespace MvvmLib.Navigation
{
    internal enum ResolutionType
    {
        Singleton,
        Existing,
        New
    }

    internal struct ViewOrObjectInstanceResult
    {
        public ResolutionType ResolutionType { get; }
        public object Instance { get; }

        public ViewOrObjectInstanceResult(ResolutionType resolutionType, object instance)
        {
            this.ResolutionType = resolutionType;
            this.Instance = instance;
        }
    }
}

