using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Samovar.DataGrid.Data.Interface;
using Samovar.DataGrid.Data.Service;
using System;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class SamovarGridBase
        : ComponentBase
    {
        internal readonly Guid ComponentId = Guid.NewGuid();
        internal ILifetimeScope ContainerLifetimScope { get; private set; }
        protected override void OnInitialized()
        {
            //MichelServiceRegisterer reg = new MichelServiceRegisterer(ComponentId);
            ContainerLifetimScope = MichelServiceRegisterer.Container.BeginLifetimeScope(
              b =>
              {
                  b.RegisterType<MichelService>().As<IMichelService>().InstancePerDependency();
                  //b.Register(ctx => new Dependency(ComponentId.ToString()));
              });
            base.OnInitialized();
        }

        //public override Task SetParametersAsync(ParameterView parameters)
        //{
        //    InitializeDependencies(in parameters);
        //    //if (!skipBaseSetParametersAsync)
        //    {
        //        return base.SetParametersAsync(parameters);
        //    }
        //    //return Task.CompletedTask;
        //}

        //protected override void BuildRenderTree(RenderTreeBuilder builder)
        //{
        //    builder.AddCascadingValue
        //    base.BuildRenderTree(builder);
        //}
        //private void InitializeDependencies(in ParameterView parameters)
        //{
        //    if (!serviceProviderInitialized)
        //    {
        //        serviceProviderInitialized = true;
        //        ServiceProviderAccessor = parameters.GetValueOrDefault("ServiceProviderAccessor", ServiceProviderAccessor);
        //        if (ServiceProviderAccessor != null)
        //        {
        //            ServiceProvider.GetService<IComponentServiceFactory>().ResolveDependencies(this);
        //        }
        //        InitializeLifetimeProperties(ServiceProvider?.Cancellation ?? CancellationToken.None);
        //        OnModelResolved(in parameters);
        //    }
        //}
    }
}

public class Dependency
{
    public string Name { get; }

    public Dependency(string name)
    {
        this.Name = name;
    }
}

public class MichelService
    : IMichelService
{
    private readonly Dependency _dep;
    public string Name => this._dep.Name;

    public MichelService()
    {

    }
    public MichelService(Dependency dep)
    {
        this._dep = dep;
    }

    public string MyValue { get; set; } = "test";

    public string GetValue()
    {
        return "my value";
    }
}

internal static class MichelServiceRegisterer
{
    internal static IContainer Container { get; set; }

    //private string RootLifetimeTag = "mysrv";
    //public MichelServiceRegisterer(Guid componentId)
    //{
    //    if (Container == null)
    //        Container = Register();
    //}
    internal static void Register()
    {
        //RootLifetimeTag = Guid.NewGuid().ToString().Replace("-", "");

        var serviceCollection = new ServiceCollection();

        var containerBuilder = new ContainerBuilder();
        containerBuilder.RegisterType<MichelService>().InstancePerLifetimeScope().PropertiesAutowired();
        
        //containerBuilder.Register(ctx => new Dependency("root"));
        //containerBuilder.RegisterType<MichelService>().As<IMichelService>().SingleInstance();// InstancePerMatchingLifetimeScope(RootLifetimeTag);
        var container = containerBuilder.Build();

        // using (var scope = container.BeginLifetimeScope(RootLifetimeTag, b =>
        //{
        //    b.Populate(serviceCollection, RootLifetimeTag);
        //}))
        // {
        //     // This service provider will have access to global singletons
        //     // and registrations but the "singletons" for things registered
        //     // in the service collection will be "rooted" under this
        //     // child scope, unavailable to the rest of the application.
        //     //
        //     // Obviously it's not super helpful being in this using block,
        //     // so likely you'll create the scope at app startup, keep it
        //     // around during the app lifetime, and dispose of it manually
        //     // yourself during app shutdown.
        //     var serviceProvider = new AutofacServiceProvider(scope);
        // }

        // Set the service locator to an AutofacServiceLocator.
        var csl = new AutofacServiceLocator(container);
        ServiceLocator.SetLocatorProvider(() => csl);

        //return container;

        Container = container;
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class SmInjectAttribute : Attribute
    {
    }
}