using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.DataGrid.Data.Interface
{
    public interface IMichelService
    {
        string MyValue { get; set; }
        string GetValue();
    }
}
