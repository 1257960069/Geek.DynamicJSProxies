using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Geek.DynamicJSProxies.AspNetCore.Mvc.Proxying.Utils;
using Geek.DynamicJSProxies.Web.Api.Modeling;
using Geek.DynamicJSProxies.Web.Api.ProxyScripting.Configuration;

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Geek.DynamicJSProxies.AspNetCore.Mvc.Proxying
{
    public class AspNetCoreApiDescriptionModelProvider
    {
        private readonly ILogger _logger;

        private readonly IApiDescriptionGroupCollectionProvider _descriptionProvider;
        private readonly ApiProxyScriptingConfiguration _apiProxyScriptingConfiguration;

        public AspNetCoreApiDescriptionModelProvider(
            IApiDescriptionGroupCollectionProvider descriptionProvider,
            ApiProxyScriptingConfiguration apiProxyScriptingConfiguration)
        {
            _descriptionProvider = descriptionProvider;
            _apiProxyScriptingConfiguration = apiProxyScriptingConfiguration;

            _logger = NullLogger.Instance;
        }

        public ApplicationApiDescriptionModel CreateModel()
        {
            var model = new ApplicationApiDescriptionModel();

            foreach (var descriptionGroupItem in _descriptionProvider.ApiDescriptionGroups.Items)
            {
                foreach (var apiDescription in descriptionGroupItem.Items)
                {
                    if (!(apiDescription.ActionDescriptor is ControllerActionDescriptor))
                    {
                        continue;
                    }

                    AddApiDescriptionToModel(apiDescription, model);
                }
            }

            return model;
        }

        private void AddApiDescriptionToModel(ApiDescription apiDescription, ApplicationApiDescriptionModel model)
        {
            var moduleModel = model.GetOrAddModule(GetModuleName(apiDescription));
            var controllerModel = moduleModel.GetOrAddController(GetControllerName(apiDescription));

            var method = (apiDescription.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            var methodName = GetNormalizedMethodName(controllerModel, method);

            if (controllerModel.Actions.ContainsKey(methodName))
            {
                _logger.LogWarning($"Controller '{controllerModel.Name}' contains more than one action with name '{methodName}' for module '{moduleModel.Name}'. Ignored: " + method);
                return;
            }

            var returnValue = new ReturnValueApiDescriptionModel(method.ReturnType);

            var actionModel = controllerModel.AddAction(new ActionApiDescriptionModel(
                methodName,
                returnValue,
                apiDescription.RelativePath,
                apiDescription.HttpMethod
            ));

            AddParameterDescriptionsToModel(actionModel, method, apiDescription);
        }
        public static bool IsAsync(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            );
        }
        private string GetNormalizedMethodName(ControllerApiDescriptionModel controllerModel, MethodInfo method)
        {

            if (!IsAsync(method))
            {
                return method.Name;
            }

            var normalizedName =Regex.Replace(method.Name, "Async$","");
            if (controllerModel.Actions.ContainsKey(normalizedName))
            {
                return method.Name;
            }

            return normalizedName;
        }

        private static string GetControllerName(ApiDescription apiDescription)
        {
            return apiDescription.GroupName ?? (apiDescription.ActionDescriptor as ControllerActionDescriptor)?.ControllerName;
        }

        private void AddParameterDescriptionsToModel(ActionApiDescriptionModel actionModel, MethodInfo method, ApiDescription apiDescription)
        {
            if (!apiDescription.ParameterDescriptions.Any())
            {
                return;
            }

            var matchedMethodParamNames = ArrayMatcher.Match(
                apiDescription.ParameterDescriptions.Select(p => p.Name).ToArray(),
                method.GetParameters().Select(GetMethodParamName).ToArray()
            );

            for (var i = 0; i < apiDescription.ParameterDescriptions.Count; i++)
            {
                var parameterDescription = apiDescription.ParameterDescriptions[i];
                var matchedMethodParamName = matchedMethodParamNames.Length > i
                                                 ? matchedMethodParamNames[i]
                                                 : parameterDescription.Name;

                actionModel.AddParameter(new ParameterApiDescriptionModel(
                        parameterDescription.Name,
                        matchedMethodParamName,
                        parameterDescription.Type,
                        parameterDescription.RouteInfo?.IsOptional ?? false,
                        parameterDescription.RouteInfo?.DefaultValue,
                        parameterDescription.RouteInfo?.Constraints?.Select(c => c.GetType().Name).ToArray(),
                        parameterDescription.Source.Id
                    )
                );
            }
        }

        public string GetMethodParamName(ParameterInfo parameterInfo)
        {
            var modelNameProvider = parameterInfo.GetCustomAttributes()
                .OfType<IModelNameProvider>()
                .FirstOrDefault();

            if (modelNameProvider == null)
            {
                return parameterInfo.Name;
            }

            return modelNameProvider.Name;
        }

        private string GetModuleName(ApiDescription apiDescription)
        {
            var controllerType = (apiDescription.ActionDescriptor as ControllerActionDescriptor)?.ControllerTypeInfo.AsType();
            if (controllerType == null)
            {
                return "app";
            }

            //foreach (var controllerSetting in _configuration.ControllerAssemblySettings.Where(setting => setting.TypePredicate(controllerType)))
            //{
            //    if (Equals(controllerType.GetAssembly(), controllerSetting.Assembly))
            //    {
            //        return controllerSetting.ModuleName;
            //    }
            //}

            return "app";
        }
    }
}
