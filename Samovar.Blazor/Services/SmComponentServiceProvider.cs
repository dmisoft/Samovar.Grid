using Microsoft.Extensions.DependencyInjection;
using System;

namespace Samovar.Blazor
{
    public class SmComponentServiceProvider
    {
        IServiceScope? _scope;

        public void InitServices<T>()
        {
            ServiceCollection _services;
            ServiceProvider _serviceProvider;

            _services = new ServiceCollection();
            _services.AddScoped<IColumnService, ColumnService>();
            _services.AddScoped<IRepositoryService<T>, RepositoryService<T>>();
            _services.AddScoped<INavigationService, NavigationService<T>>();
            _services.AddScoped<ILayoutService, LayoutService>();
            _services.AddScoped<IEditingService<T>, EditingService<T>>();
            _services.AddScoped<IInitService, InitService>();
            _services.AddScoped<IGridStateService, GridStateService>();
            _services.AddScoped<IJsService, JsService>();
            _services.AddScoped<IFilterService, FilterService>();
            _services.AddScoped<ISortingService, SortingService>();
            _services.AddScoped<IModelFactoryService, ModelFactoryService>();
            _services.AddScoped<ITemplateService<T>, TemplateService<T>>();
            
            _services.AddScoped<IVirtualScrollingNavigationStrategy, VirtualScrollingNavigationStrategy<T>>();
            _services.AddScoped<IPagingNavigationStrategy, PagingNavigationStrategy<T>>();

            _services.AddScoped<IConstantService, ConstantService>();
            _services.AddScoped<IColumnResizingService, ColumnResizingService>();
            _services.AddScoped<IGridSelectionService<T>, GridSelectionService<T>>();
            _services.AddScoped<IComponentBuilderService, ComponentBuilderService<T>>();
            _services.AddScoped<IRowDetailService<T>, RowDetailService<T>>();
            _services.AddScoped<IDataSourceService<T>, DataSourceService<T>>();

            _services.AddTransient<IDataColumnModel, DataColumnModel>();
            _services.AddTransient<ICommandColumnModel, CommandColumnModel>();

            _serviceProvider = _services.BuildServiceProvider(validateScopes: true);
            _scope = _serviceProvider.CreateScope();
        }
        public object GetService(Type serviceType)
        {
            if(_scope is null)
            {
                throw new InvalidOperationException("Services not initialized");
            }
            object retVal;
            retVal = _scope.ServiceProvider.GetRequiredService(serviceType);
            return retVal;
        }
    }
}
