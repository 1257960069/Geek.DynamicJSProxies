using System;
using System.Linq;
using System.Collections.Generic;

namespace Geek.DynamicJSProxies.Web.Api.Modeling
{
    [Serializable]
    public class ModuleApiDescriptionModel
    {
        public string Name { get; set; }

        public IDictionary<string, ControllerApiDescriptionModel> Controllers { get; }

        private ModuleApiDescriptionModel()
        {
            
        }

        public ModuleApiDescriptionModel(string name)
        {
            Name = name;

            Controllers = new Dictionary<string, ControllerApiDescriptionModel>();
        }

        public ControllerApiDescriptionModel AddController(ControllerApiDescriptionModel controller)
        {
            if (Controllers.ContainsKey(controller.Name))
            {
                throw new Exception($"There is already a controller with name: {controller.Name} in module: {Name}");
            }

            return Controllers[controller.Name] = controller;
        }

        public ControllerApiDescriptionModel GetOrAddController(string name)
        {
            ControllerApiDescriptionModel obj;
            if (Controllers.TryGetValue(name, out obj))
            {
                return obj;
            }
            return Controllers[name] = new ControllerApiDescriptionModel(name);
        }
        
        public ModuleApiDescriptionModel CreateSubModel(string[] controllers, string[] actions)
        {
            var subModel = new ModuleApiDescriptionModel(Name);

            foreach (var controller in Controllers.Values)
            {
                if (controllers == null || controllers.Contains(controller.Name))
                {
                    subModel.AddController(controller.CreateSubModel(actions));
                }
            }

            return subModel;
        }
    }
}