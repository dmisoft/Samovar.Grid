using System;
using System.Collections.Generic;
using System.Text;

namespace Samovar.DataGrid
{
    public class GridColumnDragDropService
    {
        public object Data { get; set; }
        public string Zone { get; set; }

        public void StartDrag(object data, string zone)
        {
            Data = data;
            Zone = zone;
        }

        public bool Accepts(string zone)
        {
            return Zone == zone;
        }
    }
}
