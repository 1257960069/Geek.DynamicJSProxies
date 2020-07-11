using System.Linq;

using Geek.DynamicJSProxies.Extensions;
using Geek.DynamicJSProxies.Web.Api.ProxyScripting;
using Geek.DynamicJSProxies.Web.Api.ProxyScripting.Generators.JQuery;

namespace Geek.DynamicJSProxies.AspNetCore.Mvc.Proxying
{
    public class ApiProxyGenerationModel
    {
        public string Type { get; set; } = JQueryProxyScriptGenerator.Name;

        public bool UseCache { get; set; } = true;

        public string Modules { get; set; }

        public string Controllers { get; set; }

        public string Actions { get; set; }

        public ApiProxyGenerationOptions CreateOptions()
        {
            var options = new ApiProxyGenerationOptions(Type, UseCache);

            if (!string.IsNullOrWhiteSpace(Modules))
            {
                options.Modules = Modules.Split('|').Select(m => m.Trim()).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Controllers))
            {
                options.Controllers = Controllers.Split('|').Select(m => m.Trim()).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Actions))
            {
                options.Actions = Actions.Split('|').Select(m => m.Trim()).ToArray();
            }

            return options;
        }
    }
}