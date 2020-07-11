using Geek.DynamicJSProxies.Web.Api.ProxyScripting;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Geek.DynamicJSProxies.AspNetCore.Mvc.Proxying
{
    public class AbpServiceProxiesController : Controller
    {
        [HttpGet]
        [Produces("application/x-javascript")]
        public ContentResult GetAll([FromQuery]ApiProxyGenerationModel model,[FromServices] ApiProxyScriptManager _proxyScriptManager)
        {
            var script = _proxyScriptManager.GetScript(model.CreateOptions());
            return Content(script, "application/x-javascript");
        }
    }
}
