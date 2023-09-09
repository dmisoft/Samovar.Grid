using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Samovar.DataGrid.Data.Interface;
using Samovar.DataGrid.Data.Service;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Samovar.DataGrid
{

    public static class DataGridDIExtension
    {
        internal static IContainer ApplicationContainer { get; set; }
        public static void AddSamovarDataGrid(this IServiceCollection services)
        {
            MichelServiceRegisterer.Register();
            //services.AddTransient<SamovarDataGridService>();
            //services.AddTransient<IMichelService, MichelService>();
            //services.Add<GridColumnService>()
            //var builder = new ContainerBuilder();
            //builder.Populate(services);

            //var builder = new ContainerBuilder();
            //builder.Populate(services);

            //// use your property selector to discover the properties marked with [inject]
            //builder.RegisterType<MichelService>().PropertiesAutowired((new InjectablePropertySelector(true)));

            //ApplicationContainer = builder.Build();
            //return new AutofacServiceProvider(ApplicationContainer);
        }
    }

    public static class QueryableExt
    {
        public static IQueryable<IGrouping<TKey, TElement>> GroupByProps<TElement, TKey>(this IQueryable<TElement> self, TKey model, params string[] propNames)
        {
            var modelType = model.GetType();
            var props = modelType.GetProperties();
            var types = props.Select(t => t.PropertyType).ToArray();
            var modelCtor = modelType.GetConstructor(types);
            
            var dict = new Dictionary<string, object> { { "Property", "foo" } };
            var eo = new ExpandoObject();
            
            var eoColl = (ICollection<KeyValuePair<string, object>>)eo;

            foreach (var kvp in dict)
            {
                eoColl.Add(kvp);
            }
            dynamic eoDynamic = eo;
            string value = eoDynamic.Property;



            return self.GroupByProps(model, modelCtor, props, propNames);
        }

        private static IQueryable<IGrouping<TKey, TElement>> GroupByProps<TElement, TKey>(this IQueryable<TElement> self, TKey model, ConstructorInfo modelCtor, PropertyInfo[] props, params string[] propNames)
        {
            var parameter = Expression.Parameter(typeof(TElement), "r");
            var propExpressions = props
                .Select(p =>
                {
                    Expression value;

                    if (propNames.Contains(p.Name))
                        value = Expression.PropertyOrField(parameter, p.Name);
                    else
                        value = Expression.Convert(Expression.Constant(null, typeof(object)), p.PropertyType);

                    return value;
                })
                .ToArray();

            var n = Expression.New(
                modelCtor,
                propExpressions,
                props
            );

            var expr = Expression.Lambda<Func<TElement, TKey>>(n, parameter);
            return self.GroupBy(expr);
        }
    }
}
