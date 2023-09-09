using Autofac;
using CommonServiceLocator;
using Microsoft.AspNetCore.Components;
using Samovar.DataGrid.Data.Interface;
using Samovar.DataGrid.Data.Service;
using System;

namespace Samovar.DataGrid
{
    public partial class GridPagingFooter<TItem>
        : IDisposable
    {

        //[Inject]
        //IMichelService MichelService { get; set; }

        [CascadingParameter(Name = "datagrid-container")]
        protected SamovarGrid<TItem> DataGrid { get; set; }

        MichelService srv;
        protected override void OnInitialized()
        {
            //var child2 = MichelServiceRegisterer.Container.BeginLifetimeScope(
            //  b =>
            //  {
            //      b.RegisterType<MichelService>().As<IMichelService>().SingleInstance();
            //      b.Register(ctx => new Dependency(DataGrid.ComponentId.ToString()));
            //  });
            srv = DataGrid.ContainerLifetimScope.Resolve<MichelService>();

            //srv = (MichelService)ServiceLocator.Current.GetInstance<IMichelService>();

            //var scope = MichelServiceRegisterer.Container.BeginLifetimeScope(DataGrid.ComponentId);
            //srv = (MichelService)scope.Resolve<IMichelService>();


            base.OnInitialized();
        }

        public void Dispose()
        {
            //this.grid.NotifierService.NotifyAfterScroll -= NotifierService_NotifyAfterScroll;
        }
    }
}
