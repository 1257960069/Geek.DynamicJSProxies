# Geek.DynamicJSProxies

`Geek.DynamicJSProxies` 是一个动态生成Jquery的组件，您可以通过 JavaScript 中的 ajax 使用动态创建的 Web api 控制器。ASP.NET样板还通过为动态 Web api 控制器创建动态 JavaScript 代理来简化这一点。您可以像函数调用一样从 JavaScript 调用动态 Web api 控制器的操作：

应用场景：前端无需编写请求代码。

[![Latest version](https://img.shields.io/nuget/v/Panda.DynamicWebApi.svg)](https://www.nuget.org/packages/Geek.DynamicJSProxies/)

## 1.快速入门

（1）新建一个 ASP.NET Core WebApi(或MVC) 项目

（2）通过Nuget安装组件

````shell
Install-Package Geek.DynamicJSProxies
````

（3）在 Startup 中注册 DynamicWebApi

````csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDynamicJSProxies();
}
````

（4）前端使用

```
<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title></title>
</head>
<body>
    <script src="https://cdn.bootcdn.net/ajax/libs/jquery/3.5.1/jquery.js"></script>
    <script src="/Abp/Framework/scripts/abp.js" type="text/javascript"></script>
    <script src="/Abp/Framework/scripts/libs/abp.jquery.js" type="text/javascript"></script>
    <script src="/AbpServiceProxies/GetAll?type=jquery" type="text/javascript"></script>
</body>
</html>
```

（6）运行

运行浏览器以后访问 `<你的项目地址>/AbpServiceProxies/GetAll`，将会看到为我们 `Geek.DynamicJSProxies` 生成的 

本快速入门 Demo 地址：[点我](/samples/Geek.DynamicJSProxies.Samples)

## 2.更进一步



复制
abp.services.tasksystem.task.getTasks({
    state: 1
}).done(function (result) {
    //use result.tasks here...
});
JavaScript 代理是动态创建的。在使用动态脚本之前，应在页面上包含该动态脚本：

复制

```
<script src="/api/AbpServiceProxies/GetAll" type="text/javascript"></script>
```

服务方法返回承诺（请参阅jQuery.递延）。你可以注册到完成， 失败， 然后...回调。在内部，服务方法使用abp.ajax。它们处理错误，并根据需要显示错误消息。

AJAX 参数
您可能希望将自定义 ajax 参数传递给代理方法。您可以将它们作为第二个参数传递，如下所示：

复制

```
abp.services.tasksystem.task.createTask({
    assignedPersonId: 3,
    description: 'a new task description...'
},{ //override jQuery's ajax parameters
    async: false,
    timeout: 30000
}).done(function () {
    abp.notify.success('successfully created a task!');
});
```

jQuery.ajax 的所有参数都在这里有效。

除了标准 jQuery.ajax 参数外，您还可以向 AJAX 选项添加abpHandleError：false，以便禁用错误发生时显示的消息。

单一服务脚本
'/api/AbpServiceProxies/GetAll'在一个文件中生成所有服务代理。您还可以使用"/api/AbpServiceProxies/获取名称=服务名称"生成单个服务代理，并在页面中包含脚本，如下所示：

复制

```
<script src="/api/AbpServiceProxies/Get?name=tasksystem/task" type="text/javascript"></script>
```

角JS集成
ASP.NET样板可以公开动态api控制器作为角js服务。请考虑以下示例：

复制

```
(function() {
    angular.module('app').controller('TaskListController', [
        '$scope', 'abp.services.tasksystem.task',
        function($scope, taskService) {
            var vm = this;
            vm.tasks = [];
            taskService.getTasks({
                state: 0
            }).then(function(result) {
                vm.tasks = result.data.tasks;
            });
        }
    ]);
})();
```

我们可以使用服务的名称（使用命名空间）注入服务。我们可以把它的函数称为常规的JavaScript函数。请注意，我们注册到然后处理程序（而不是完成），因为它类似于角的$http服务。ASP.NET锅炉板使用$http的公共服务。如果要传递配置$http，可以传递配置对象作为服务方法的最后一个参数。

为了能够使用自动生成的服务，您应该在页面上包括这些所需的脚本：

复制

```
<script src="~/Abp/Framework/scripts/libs/angularjs/abp.ng.js"></script>
<script src="~/api/AbpServiceProxies/GetAll?type=angular"></script>
```

## 3.配置

所有的配置均在对象 `DynamicWebApiOptions` 中，说明如下：

| 属性名                      | 是否必须 | 说明                                                      |
| --------------------------- | -------- | --------------------------------------------------------- |
| Type             | 否       | 默认值：jquery。                                |
| UseCache             | 否       | 默认值：true。使用缓存                                 |
| Controllers          | 否       | 默认值：空。只获取需要某些Controller下的代理,多个以"\|"分割 |
| Actions          | 否       | 默认值：Async。只获取需要某些Action下的代理,多个以"\|"分割 |


## 4.疑难解答

若遇到问题，可使用 [Issues](https://github.com/1257960069/Geek.DynamicJSProxies/issues) 进行提问。

## 5.引用项目说明

> 本项目直接或间接引用了以下项目

- [ABP](https://github.com/aspnetboilerplate/aspnetboilerplate)

