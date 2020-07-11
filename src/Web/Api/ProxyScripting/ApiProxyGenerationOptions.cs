using System.Collections.Generic;

namespace Geek.DynamicJSProxies.Web.Api.ProxyScripting
{
    public class ApiProxyGenerationOptions
    {
        public string GeneratorType { get; set; }

        public bool UseCache { get; set; }

        public string[] Modules { get; set; }

        public string[] Controllers { get; set; }

        public string[] Actions { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public ApiProxyGenerationOptions(string generatorType, bool useCache = true)
        {
            GeneratorType = generatorType;
            UseCache = useCache;

            Properties = new Dictionary<string, string>();
        }

        public bool IsPartialRequest()
        {
            return Modules?.Length>0 || Controllers?.Length > 0 || Actions?.Length > 0;
        }
    }
}