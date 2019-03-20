using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace MvvmLib.Services
{
    public interface IDataContractJsonSerializerService
    {
        T Parse<T>(string json, DataContractJsonSerializerSettings settings);
        T Parse<T>(string json, List<Type> knownTypes);
        List<Type> ResolveTypes(Type type, List<Type> knownTypes = null);
        string Stringify(object obj, DataContractJsonSerializerSettings settings);
        string Stringify(object obj, List<Type> knownTypes);
        bool TryParse(Type type, string json, out object result);
        bool TryParse<T>(string json, out T result);
        bool TryStringify(object obj, out string result);
    }
}