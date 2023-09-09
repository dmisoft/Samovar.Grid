using Microsoft.AspNetCore.Components;
using System;
using System.Reflection;

namespace Samovar.DataGrid
{
    public partial class GridEditCellNumeric<TValue, TEntity>
        //: ComponentBase
    {
        [Parameter]
        public object Data { get; set; }

        [Parameter]
        public PropertyInfo PropInfo { get; set; }

        private TValue innerValue;
        protected TValue InnerValue
        {
            set
            {
                innerValue = value;
                PropInfo.SetValue(Data, innerValue);
            }
            get
            {
                return innerValue;
            }
        }


        protected override void OnInitialized()
        {
            InnerValue = (TValue)PropInfo.GetValue(Data) == null ? default : (TValue)PropInfo.GetValue(Data);
        }

        public void InnerValueOnChange(ChangeEventArgs args)
        {
            PropInfo.SetValue(Data, innerValue);
        }
    }
}
