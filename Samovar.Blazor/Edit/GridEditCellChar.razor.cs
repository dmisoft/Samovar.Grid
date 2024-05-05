using Microsoft.AspNetCore.Components;
using System;
using System.Reflection;

namespace Samovar.Blazor.Edit
{
    public partial class GridEditCellChar
    {
        [Parameter]
        public required object Data { get; set; }

        [Parameter]
        public required PropertyInfo PropInfo { get; set; }

        private char innerValue;
        protected char InnerValue
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
            InnerValue = (char)PropInfo.GetValue(Data);
        }
       
        public void InnerValueOnChange(ChangeEventArgs args)
        {
            PropInfo.SetValue(Data, innerValue);
        }
    }
}
