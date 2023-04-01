using Asoode.Core.Contracts.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Asoode.Backend.Engine
{
    internal class JsonBiz : IJsonBiz
    {
        private readonly JsonSerializerSettings _settings;

        public JsonBiz()
        {
            _settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }
    }
}