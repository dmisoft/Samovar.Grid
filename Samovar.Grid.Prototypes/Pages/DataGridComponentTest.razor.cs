using Microsoft.AspNetCore.Components;
using Samovar.Grid.Prototypes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.Grid.Prototypes.Pages
{
    public partial class DataGridComponentTest
    {
        List<WeatherForecast> data;

        int currentCount;
        int currentCountForStrData;
        protected override async Task OnInitializedAsync()
        {
            data = (await DataService.GetForecastAsync(DateTime.Now)).OrderBy(i => i.Position).ToList();
        }

        void ChangeElementAt0()
        {
            data[0].Summary = "Hallo";
        }

        void RemoveElementAt0()
        {
            data.RemoveAt(0);
        }

        void LastingTest()
        {
            Task.Run(async () =>
            {
                var rng = new Random();
                for (int i = 1; i <= 500; i++)
                {
                    data = (await DataService.GetForecastAsync(DateTime.Now)).ToList();
                    await InvokeAsync(StateHasChanged);
                    System.Threading.Thread.Sleep(50);
                }
            });
        }


    }
}
