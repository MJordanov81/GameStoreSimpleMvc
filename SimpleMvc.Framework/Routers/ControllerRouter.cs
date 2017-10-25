namespace SimpleMvc.Framework.Routers
{
    using Attributes;
    using Attributes.Authentication;
    using Attributes.Methods;
    using Contracts;
    using Controllers;
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using WebServer.Contracts;
    using WebServer.Exceptions;
    using WebServer.Http.Contracts;
    using WebServer.Http.Response;

    public class ControllerRouter : IHandleable
    {
        private IDictionary<string, string> getParameters;
        private IDictionary<string, string> postParameters;
        private IDictionary<Type, Type> interfaceDefaultImplementations;
        private string requestMethod;
        private Controller controllerInstance;
        private string controllerName;
        private string actionName;
        private object[] methodParameters;

        public ControllerRouter(IDictionary<Type, Type> interfaceDefaultImplementations)
        {
            this.interfaceDefaultImplementations = interfaceDefaultImplementations;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            this.controllerInstance = null;
            this.actionName = null;
            this.methodParameters = null;

            this.getParameters = new Dictionary<string, string>(request.UrlParameters);
            this.postParameters = new Dictionary<string, string>(request.FormData);
            this.requestMethod = request.Method.ToString().ToUpper();

            this.PrepareControllerAndActionNames(request);

            var methodInfo = this.GetActionForExecution();

            if (methodInfo == null)
            {
                return new NotFoundResponse();
            }

            this.PrepareMethodParameters(methodInfo);

            try
            {
                if (this.controllerInstance != null)
                {
                    this.controllerInstance.Request = request;
                    this.controllerInstance.InitializeController();
                }

                return this.GetResponse(methodInfo, this.controllerInstance);
            }
            catch (Exception ex)
            {
                return new InternalServerErrorResponse(ex);
            }
        }

        private void PrepareControllerAndActionNames(IHttpRequest request)
        {
            var pathParts = request.Path.Split(
                new[] { '/', '?' },
                StringSplitOptions.RemoveEmptyEntries);

            if (pathParts.Length < 2)
            {
                if (request.Path == "/")
                {
                    this.controllerName = "HomeController";
                    this.actionName = "Index";

                    return;
                }
                else
                {
                    BadRequestException.ThrowFromInvalidRequest();
                }
            }

            this.controllerName = $"{pathParts[0].Capitalize()}{MvcContext.Get.ControllerSuffix}";
            this.actionName = pathParts[1].Capitalize();
        }

        private MethodInfo GetActionForExecution()
        {
            foreach (var method in this.GetSuitableMethods())
            {
                var httpMethodAttributes = method
                    .GetCustomAttributes()
                    .Where(a => a is HttpMethodAttribute)
                    .Cast<HttpMethodAttribute>();

                if (!httpMethodAttributes.Any() && this.requestMethod == "GET")
                {
                    return method;
                }

                foreach (var httpMethodAttribute in httpMethodAttributes)
                {
                    if (httpMethodAttribute.IsValid(this.requestMethod))
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods()
        {
            var controller = this.GetControllerInstance();

            if (controller == null)
            {
                return new MethodInfo[0];
            }

            return controller
                .GetType()
                .GetMethods()
                .Where(m => m.Name.ToLower() == actionName.ToLower());
        }

        private object GetControllerInstance()
        {
            if (this.controllerInstance != null)
            {
                return controllerInstance;
            }

            var controllerFullQualifiedName = string.Format(
                "{0}.{1}.{2}, {0}",
                MvcContext.Get.AssemblyName,
                MvcContext.Get.ControllersFolder,
                this.controllerName);

            var controllerType = Type.GetType(controllerFullQualifiedName);

            if (controllerType == null)
            {
                return null;
            }

            this.controllerInstance = Activator.CreateInstance(controllerType) as Controller;


            this.InjectControllerDependencies(this.controllerInstance);


            return this.controllerInstance;
        }

        private void InjectControllerDependencies(object instance)
        {
            FieldInfo[] fields = instance
                .GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetCustomAttributes()
                .Any(a => a.GetType() == typeof(InjectAttribute)))
                .ToArray();

            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsInterface && !field.FieldType.IsAbstract)
                {
                    object fieldInstance = Activator.CreateInstance(field.FieldType);

                    field.SetValue(instance, fieldInstance);

                    InjectControllerDependencies(fieldInstance);
                    continue;
                }

                Type[] appTypes = Assembly
                    .GetEntryAssembly()
                    .GetTypes()
                    .Where(t => field.FieldType.IsAssignableFrom(t) && !t.IsInterface)
                    .ToArray();

                if (appTypes.Length > 1)
                {
                    if (!this.interfaceDefaultImplementations.ContainsKey(field.FieldType))
                    {
                        throw new InvalidOperationException($"Please specify default implementation for interface {field.FieldType}!");
                    }

                    object fieldInstance = Activator.CreateInstance(this.interfaceDefaultImplementations[field.FieldType]);

                    field.SetValue(instance, fieldInstance);

                    InjectControllerDependencies(fieldInstance);

                    continue;
                }

                else
                {
                    object fieldInstance = Activator.CreateInstance(appTypes[0]);

                    field.SetValue(instance, fieldInstance);

                    InjectControllerDependencies(fieldInstance);
                }
            }
        }

        private void PrepareMethodParameters(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            this.methodParameters = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                if (parameter.ParameterType.IsPrimitive
                    || parameter.ParameterType == typeof(string))
                {
                    this.ProcessPrimitiveParameter(parameter, i);
                }
                else
                {
                    this.ProcessModelParameter(parameter, i);
                }
            }
        }

        private void ProcessPrimitiveParameter(ParameterInfo parameter, int index)
        {
            var getParameterValue = this.getParameters[parameter.Name];

            var value = Convert.ChangeType(
                getParameterValue,
                parameter.ParameterType);

            this.methodParameters[index] = value;
        }

        private void ProcessModelParameter(ParameterInfo parameter, int index)
        {
            var modelType = parameter.ParameterType;
            var modelInstance = Activator.CreateInstance(modelType);

            var modelProperties = modelType.GetProperties();

            foreach (var modelProperty in modelProperties)
            {
                var postParameterValue = this.postParameters[modelProperty.Name];

                var value = Convert.ChangeType(
                    postParameterValue,
                    modelProperty.PropertyType);

                modelProperty.SetValue(
                    modelInstance,
                    value);
            }

            this.methodParameters[index] = Convert.ChangeType(
                modelInstance,
                modelType);
        }

        private IHttpResponse GetResponse(MethodInfo method, object controller)
        {
            bool isUserAuthorizedForAction = CheckIfUserIsAuthorized(method, controller);

            if (!isUserAuthorizedForAction)
            {
                return new ForbiddenResponse();
            }

            var actionResult = method.Invoke(controller, this.methodParameters)
                as IActionResult;

            if (actionResult == null)
            {
                var methodResultAsHttpResponse = actionResult as IHttpResponse;

                if (methodResultAsHttpResponse != null)
                {
                    return methodResultAsHttpResponse;
                }
                else
                {
                    throw new InvalidOperationException("Controller actions should return either IActionResult or IHttpResponse.");
                }
            }

            return actionResult.Invoke();
        }

        private bool CheckIfUserIsAuthorized(MethodInfo method, object controller)
        {
            bool isUserAdmin = (controller as Controller).User.IsAdmin;

            if (isUserAdmin)
            {
                return true;
            }
            else
            {
                bool isUserAuthenticated = (controller as Controller).User.IsAuthenticated;

                IList<AuthenticationAttribute> attributes = method
                    .GetCustomAttributes<AuthenticationAttribute>()
                    .ToList();

                if (attributes.Any(a => a.GetType() == typeof(AdminAttribute)))
                {
                    return false;
                }
                if (attributes.Any(a => a.GetType() == typeof(AuthenticatedAttribute)) && !isUserAuthenticated)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
