using Microsoft.AspNetCore.Components;
using System;
using System.Reflection;

namespace Samovar.Blazor.Edit
{
    public partial class GridEditCellDate
    {
        [Parameter]
        public required object Data { get; set; }

        [Parameter]
        public required PropertyInfo PropInfo { get; set; }

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
