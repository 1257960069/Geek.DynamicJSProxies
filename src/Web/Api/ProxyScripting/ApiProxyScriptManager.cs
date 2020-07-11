using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

using Geek.DynamicJSProxies.AspNetCore.Mvc.Proxying;
using Geek.DynamicJSProxies.Extensions;
using Geek.DynamicJSProxies.Web.Api.ProxyScripting.Generators.JQuery;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace Geek.DynamicJSProxies.Web.Api.ProxyScripting
{
    public class ApiProxyScriptManager
    {
        private readonly AspNetCoreApiDescriptionModelProvider _modelProvider;
        private readonly IServiceProvider _iocResolver;

        private readonly ConcurrentDictionary<string, string> _cache;

        public ApiProxyScriptManager(
            AspNetCoreApiDescriptionModelProvider modelProvider,
            IServiceProvider iocResolver)
        {
            _modelProvider = modelProvider;
            _iocResolver = iocResolver;

            _cache = new ConcurrentDictionary<string, string>();
        }

        public string GetScript(ApiProxyGenerationOptions options)
        {
            if (options.UseCache)
            {
                return _cache.GetOrAdd(CreateCacheKey(options), (key) => CreateScript(options));
            }

            return _cache[CreateCacheKey(options)] = CreateScript(options);
        }

        private string CreateScript(ApiProxyGenerationOptions options)
        {
            var model = _modelProvider.CreateModel();

            if (options.IsPartialRequest())
            {
                model = model.CreateSubModel(options.Modules, options.Controllers, options.Actions);
            }

            using (var scope = _iocResolver.CreateScope())
            {
                var generator = scope.ServiceProvider.GetRequiredService<JQueryProxyScriptGenerator>();
                return generator.CreateScript(model);
            }
        }

        private static string CreateCacheKey(ApiProxyGenerationOptions options)
        {
            var str = JsonConvert.SerializeObject(options);
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(str);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var hashByte in hashBytes)
                {
                    sb.Append(hashByte.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
