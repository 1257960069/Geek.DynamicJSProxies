using Geek.DynamicJSProxies.AspNetCore.Mvc.Proxying;
using Geek.DynamicJSProxies.Web.Api.ProxyScripting;
using Geek.DynamicJSProxies.Web.Api.ProxyScripting.Configuration;
using Geek.DynamicJSProxies.Web.Api.ProxyScripting.Generators.JQuery;

using Microsoft.Extensions.DependencyInjection;

namespace Geek.DynamicJSProxies
{
    public static class DynamicJSProxiesServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamicJSProxies(this IServiceCollection services)
        {
            services.AddSingleton<AspNetCoreApiDescriptionModelProvider>()
                .AddSingleton<ApiProxyScriptingConfiguration>()
                .AddSingleton<ApiProxyScriptManager>()
                .AddTransient<JQueryProxyScriptGenerator>();

            return services;
        }
    }
}
