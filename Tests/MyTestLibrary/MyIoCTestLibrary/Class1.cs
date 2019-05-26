using MvvmLib.IoC;
using System;

namespace MyIoCTestLibrary
{
    // A
    public interface IMyExternalService
    { }

    public class MyExternalService : IMyExternalService
    { }

    // B
    public interface IMyExternalServiceB
    { }

    public class MyExternalServiceB1 : IMyExternalServiceB
    { }

    [PreferredImplementation]
    public class MyExternalServiceB2 : IMyExternalServiceB
    { }

    public class MyExternalServiceB3 : IMyExternalServiceB
    { }

    // C

    public interface IMyExternalServiceC
    { }

    public class MyExternalServiceC1 : IMyExternalServiceC
    { }

    public class MyExternalServiceC2 : IMyExternalServiceC
    { }
}
