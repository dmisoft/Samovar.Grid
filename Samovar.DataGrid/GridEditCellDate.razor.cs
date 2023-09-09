using Microsoft.AspNetCore.Components;
using System;
using System.Reflection;

namespace Samovar.DataGrid
{
    public partial class GridEditCellDate
        //: ComponentBase
    {
        [Parameter]
        public object Data { get; set; }

        [Parameter]
        public PropertyInfo PropInfo { get; set; }

        private DateTime innerValue = DateTime.MinValue;
        protected DateTime InnerValue {
            set {
                this.innerValue = value;
                PropInfo.SetValue(Data, innerValue);
            }
            get {
                return this.innerValue;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            innerValue = (DateTime)PropInfo.GetValue(Data);
        }
        public void InnerValueOnInput(ChangeEventArgs args)
        {
        }
        public void InnerValueOnChange(ChangeEventArgs args)
        {
            PropInfo.SetValue(Data, innerValue);
        }
    }
}
