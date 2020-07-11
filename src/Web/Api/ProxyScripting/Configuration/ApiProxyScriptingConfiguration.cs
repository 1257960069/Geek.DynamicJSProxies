using System;
using System.Collections.Generic;

namespace Geek.DynamicJSProxies.Web.Api.ProxyScripting.Configuration
{
    public class ApiProxyScriptingConfiguration
    {
        public IDictionary<string, Type> Generators { get; }= new Dictionary<string, Type>();

        public bool RemoveAsyncPostfixOnProxyGeneration { get; set; } = true;

    }
}