using Microsoft.AspNetCore.Components;
using System;
using System.Reflection;

namespace Samovar.Blazor.Edit
{
    public partial class GridEditCellBoolean
        //: ComponentBase
    {
        [Parameter]
        public object Data { get; set; }

        [Parameter]
        public PropertyInfo PropInfo { get; set; }

        private bool innerValue;
        protected bool InnerValue
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
            base.OnInitialized();
            InnerValue = (bool)PropInfo.GetValue(Data);
        }

        public void InnerValueOnChange(ChangeEventArgs args)
        {
            PropInfo.SetValue(Data, innerValue);
        }
    }
}
