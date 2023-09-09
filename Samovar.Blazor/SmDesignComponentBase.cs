using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    //public interface IComponentScope
    //{
    //    bool IsDisposed { get; }

    //    CancellationToken Cancellation { get; }
    //}
    //public interface IComponentServiceFactory : IDisposable//, IComponentService
    //{
    //    //T Create<T>(params object[] constructorArgs) where T : IComponentService;

    //    //T Create<T, TArg>(in TArg arg) where T : IComponentService;

    //    void ResolveDependencies(object target);
    //}

    //internal interface IBoundControl
    //{
    //    bool InvokeRequired { get; }

    //    bool IsHandleCreated { get; }

    //    IAsyncResult BeginInvoke(Delegate method, params object[] arguments);
    //}
    //public interface IComponentServiceProvider : IServiceScope //: IComponentScope, IDisposable
    //{
    //    T GetService<T>();
    //}
    //public interface IComponentServiceProviderBuilder
    //{
    //    IComponentServiceProvider Build();

    //    IComponentServiceProvider Build<T>();

    //    IComponentServiceProvider Build<T, TComponent>();
    //}
    //public class ComponentServiceProviderBuilder : IComponentServiceProviderBuilder
    //{
    //    private readonly IServiceProvider _serviceProvider;

    //    private readonly IBoundControl _boundControl;

    //    private readonly IServiceCollection _services;

    //    internal ComponentServiceProviderBuilder(IServiceProvider serviceProvider, IBoundControl boundControl, IServiceCollection services)
    //    {
    //        _serviceProvider = serviceProvider;
    //        _boundControl = boundControl;
    //        _services = services;
    //    }

    //    public ComponentServiceProviderBuilder(IServiceProvider serviceProvider, IServiceCollection services)
    //        : this(serviceProvider, null, services)
    //    {
    //    }

    //    IComponentServiceProvider IComponentServiceProviderBuilder.Build<T, TComponent>()
    //    {
    //        throw new NotImplementedException();
    //        //_services.TryAdd(ComponentServiceCollection<T, TComponent>.Cache.Value.ToArray());
    //        //return ComponentServiceCollection.Create(_serviceProvider, _services, _boundControl);
    //    }

    //    IComponentServiceProvider IComponentServiceProviderBuilder.Build<T>()
    //    {
    //        throw new NotImplementedException();

    //        //_services.TryAdd(ComponentServiceCollection<T>.Cache.Value.ToArray());
    //        //return ComponentServiceCollection.Create(_serviceProvider, _services, _boundControl);
    //    }

    //    IComponentServiceProvider IComponentServiceProviderBuilder.Build()
    //    {
    //        return new ComponentServiceProvider(_services, _boundControl);
    //        //throw new NotImplementedException();

    //        //_services.TryAdd(ComponentServiceCollection.Cache.Value.ToArray());
    //        //return ComponentServiceCollection.Create(_serviceProvider, _services, _boundControl);
    //    }
    //}

    //internal class ComponentServiceProvider : IComponentServiceProvider, IServiceScope, IDisposable, IComponentScope
    //{
    //    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    //    private readonly IServiceScope _scope;

    //    private readonly IBoundControl _boundControl;

    //    private bool _disposed;

    //    public CancellationToken Cancellation { get; }

    //    bool IComponentScope.IsDisposed => _disposed;

    //    IServiceProvider IServiceScope.ServiceProvider => null;//=> _scope.ServiceProvider;

    //    public ComponentServiceProvider(IServiceCollection collection, IBoundControl boundControl)
    //    {
    //        //collection.AddTransient(CreateLinkedCts);
    //        //collection.TryAddSingleton((IComponentServiceProvider)this);
    //        //collection.TryAddSingleton((IComponentScope)this);
    //        //_boundControl = boundControl;
    //        //Cancellation = _cts.Token;
    //        //Cancellation.Register(_scope = collection.BuildServiceProvider().CreateScope());
    //    }

    //    private CancellationTokenSource CreateLinkedCts(IServiceProvider arg)
    //    {
    //        return CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, default(CancellationToken));
    //    }

    //    public T GetService<T>()
    //    {
    //        return _scope.ServiceProvider.GetService<T>();
    //    }

    //    public void Dispose()
    //    {
    //        if (_disposed)
    //        {
    //            return;
    //        }
    //        _disposed = true;
    //        try
    //        {
    //            using (_cts)
    //            {
    //                if (!_cts.IsCancellationRequested)
    //                {
    //                    _cts.Cancel();
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (!(ex is ObjectDisposedException) && !(ex is TaskCanceledException))
    //            {
    //                throw;
    //            }
    //        }
    //    }
    //}
    public class SmDesignComponentBase
        : ComponentBase 
    {
        [CascadingParameter(Name = "ServiceProvider")]
        IComponentServiceProvider ServiceProvider { get; set; }

        private bool _dependenciesInitialized;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            InitializeDependencies(in parameters);
            
            //InitializeModel(in parameters);
            
            return base.SetParametersAsync(parameters);
        }

        public virtual void InitializeModel(in ParameterView parameters) { }
        //{
        //    var model = parameters.GetValueOrDefault<SmDataGridRowModel<TItem>>("ServiceProvider");
        //}

        public virtual void DependenciesInitialized() { }

        private Task InitializeDependencies(in ParameterView parameters)
        {
            if (!_dependenciesInitialized)
            {
                _dependenciesInitialized = true;

                Dictionary<string, Type> _dict = new Dictionary<string, Type>();

                PropertyInfo[] props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                try
                {
                    foreach (PropertyInfo prop in props)
                    {
                        object[] attrs = prop.GetCustomAttributes(true);
                        foreach (object attr in attrs)
                        {
                            SmInjectAttribute injectAttr = attr as SmInjectAttribute;
                            if (injectAttr != null)
                            {
                                string propName = prop.Name;
                                _dict.Add(propName, prop.PropertyType);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

                IComponentServiceProvider componentServiceProvider;

                if (this is IComponentServiceProvider)
                    componentServiceProvider = this as IComponentServiceProvider;
                else
                    componentServiceProvider = parameters.GetValueOrDefault<IComponentServiceProvider>("ServiceProvider");

                if (componentServiceProvider != null)
                {
                    foreach (var pair in _dict)
                    {
                        object service = componentServiceProvider.ServiceProvider.GetService(pair.Value);
                        PropertyInfo piShared = this.GetType().GetProperty(pair.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        piShared.SetValue(this, service);
                    }
                }
                DependenciesInitialized();
            }

            return Task.CompletedTask;
        }
    }
    //public class SmComponentBase
    // : ComponentBase, IServiceProviderFactory<IComponentServiceProviderBuilder>, IBoundControl 
    //{
    //    [CascadingParameter(Name = "ServiceProvider")]
    //    SmComponentServiceProvider ServiceProvider { get; set; }

    //    [Inject]
    //    [EditorBrowsable(EditorBrowsableState.Never)]
    //    private IServiceProvider AppServiceProvider { get; set; }


    //    public bool InvokeRequired => throw new NotImplementedException();

    //    public bool IsHandleCreated => throw new NotImplementedException();

    //    private readonly Lazy<IServiceProvider> _serviceProviderLazy;

    //    private IServiceCollection _services;
    //    private ServiceContainer _serviceContainer;
    //    private CancellationToken cancellation;
    //    private bool _dependenciesInitialized;
    //    private readonly CancellationTokenSource lifetimeTokenSource = new CancellationTokenSource();

    //    public SmComponentBase()
    //    {

    //        cancellation = lifetimeTokenSource.Token;
    //        _serviceProviderLazy = new Lazy<IServiceProvider>(new Func<IServiceProvider>(ServiceProviderInitializer), LazyThreadSafetyMode.ExecutionAndPublication);

    //        var services = new ServiceCollection();
    //        services.AddScoped<IMichelService, MichelService>();

    //        ServiceProvider serviceProvider = services.BuildServiceProvider(validateScopes: true);
    //        //using (IServiceScope scope = serviceProvider.CreateScope())
    //        //{
    //        //    // Correctly scoped resolution
    //        //    Bar correct = scope.ServiceProvider.GetRequiredService<Bar>();
    //        //}

    //        // Not within a scope, becomes a singleton
    //        //Bar avoid = serviceProvider.GetRequiredService<Bar>();

    //        //var builder = new ContainerBuilder();

    //        //// Register individual components
    //        //builder.RegisterInstance(new MichelService())
    //        //       .As<IMichelService>();

    //        //var container = builder.Build();
    //    }

    //    private IServiceProvider ServiceProviderInitializer()
    //    {
    //        IServiceProviderFactory<IComponentServiceProviderBuilder> serviceProviderFactory = this;// AppServiceProvider.FindCustomFactory() ?? this;
    //        IServiceCollection services = new ServiceCollection(); //CreateServiceContainer().AddSingleton((IObservationHub)this);
    //        services.AddScoped<IMichelService, MichelService>();
    //        IComponentServiceProviderBuilder containerBuilder = serviceProviderFactory.CreateBuilder(services);
    //        return serviceProviderFactory.CreateServiceProvider(containerBuilder);
    //    }
    //    public override Task SetParametersAsync(ParameterView parameters)
    //    {
    //        //DMi Test
    //        //var serviceProvider = _serviceProviderLazy.Value;
    //        //var serviiceScope = serviceProvider.CreateScope();
    //        //DMi Test Ende

    //        InitializeDependencies(in parameters);
    //        return base.SetParametersAsync(parameters);
    //    }

    //    private void InitializeDependencies(in ParameterView parameters)
    //    {
    //        if (!_dependenciesInitialized)
    //        {
    //            _dependenciesInitialized = true;

    //            Dictionary<string, Type> _dict = new Dictionary<string, Type>();

    //            PropertyInfo[] props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    //            foreach (PropertyInfo prop in props)
    //            {
    //                object[] attrs = prop.GetCustomAttributes(true);
    //                foreach (object attr in attrs)
    //                {
    //                    SmInjectAttribute injectAttr = attr as SmInjectAttribute;
    //                    if (injectAttr != null)
    //                    {
    //                        string propName = prop.Name;
    //                        _dict.Add(propName, prop.PropertyType);
    //                    }
    //                }
    //            }

    //            SmComponentServiceProvider smServiceProvider = parameters.GetValueOrDefault<SmComponentServiceProvider>("ServiceProvider");

    //            if (smServiceProvider != null) {
    //                foreach (var pair in _dict)
    //                {
    //                    object service = smServiceProvider.GetService(pair.Value);
    //                    PropertyInfo piShared = this.GetType().GetProperty(pair.Key, BindingFlags.NonPublic | BindingFlags.Instance);
    //                    piShared.SetValue(this, service);
    //                }
    //            }

    //            if (TryResolveServiceProvider(in parameters, out var serviceProvider))
    //            {
    //                serviceProvider.GetService<IComponentServiceFactory>().ResolveDependencies(this);
    //                //cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation, ServiceProvider.Cancellation).Token;
    //                DependenciesResolved();
    //            }
    //        }
    //    }
    //    private bool TryResolveServiceProvider(in ParameterView parameters, out IServiceProvider serviceProvider)
    //    {
    //        serviceProvider = null;
    //        serviceProvider = _serviceProviderLazy.Value;
    //        return serviceProvider != null;

    //        //IComponentServiceProvider serviceProvider2 = null;
    //        //if (TryUseExistingServiceProvider(in parameters, ref serviceProvider2) && serviceProvider2 != null)
    //        //{
    //        //    serviceProvider = serviceProvider2.ServiceProvider;
    //        //}
    //        //else
    //        //{
    //        //    serviceProvider = _serviceProviderLazy.Value;
    //        //}
    //        //return serviceProvider != null;
    //    }

    //    protected virtual bool TryUseExistingServiceProvider(in ParameterView parameters, ref IComponentServiceProvider serviceProvider)
    //    {
    //        return false;
    //    }

    //    protected virtual void DependenciesResolved()
    //    {
    //    }
    //    public IComponentServiceProviderBuilder CreateBuilder(IServiceCollection services)
    //    {
    //        //throw new NotImplementedException();
    //        services.AddScoped<IMichelService, MichelService>();
    //        return new ComponentServiceProviderBuilder(AppServiceProvider, this, services);
    //    }

    //    public IServiceProvider CreateServiceProvider(IComponentServiceProviderBuilder containerBuilder)
    //    {
    //        return BuildServiceProvider(containerBuilder).ServiceProvider;
    //    }

    //    protected virtual IComponentServiceProvider BuildServiceProvider(IComponentServiceProviderBuilder builder)
    //    {
    //        return builder.Build();
    //    }
    //    public IAsyncResult BeginInvoke(Delegate method, params object[] arguments)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
