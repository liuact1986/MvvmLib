using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using MvvmLib.Services;
using Windows.Storage;

namespace MvvmLib.Adaptive
{

    public class AdaptiveJsonFileService 
    {
        protected IDataContractJsonSerializerService serializerService;
        protected Dictionary<string, AdaptiveJsonSetting[]> cache;

        public AdaptiveJsonFileService()
        {
            this.serializerService = new DataContractJsonSerializerService();
            this.cache = new Dictionary<string, AdaptiveJsonSetting[]>();
        }

        public async Task<string> ReadFileAsync(string configFile)
        {
            var uri = new Uri(string.Format("ms-appx:///{0}", configFile));
            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var randomStream = await file.OpenReadAsync();

                using (var reader = new StreamReader(randomStream.AsStreamForRead()))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (FileNotFoundException)
            {
                throw new ArgumentException("Json file \"" + configFile + "\" not found. Set file as \"Content\" and \"copy into the output directory\" in the properties panel.");
            }
        }

        public bool IsCached(string file)
        {
            return this.cache.ContainsKey(file);
        }

        public AdaptiveJsonSetting[] GetFromCache(string file)
        {
            return this.cache[file];
        }

        public void AddToCache(string file, AdaptiveJsonSetting[] content)
        {
            this.cache[file] = content;
        }

        public async Task<AdaptiveJsonSetting[]> LoadAsync(string file)
        {
            if (this.IsCached(file))
            {
                return this.GetFromCache(file);
            }
            else
            {
                var json = await ReadFileAsync(file);
                var result = this.serializerService.Parse<AdaptiveJsonSetting[]>(json, new DataContractJsonSerializerSettings
                {
                    KnownTypes = new List<Type> { typeof(AdaptiveJsonSetting) },
                    UseSimpleDictionaryFormat = true
                });
                this.AddToCache(file, result);
                return result;
            }
        }
    }

}
