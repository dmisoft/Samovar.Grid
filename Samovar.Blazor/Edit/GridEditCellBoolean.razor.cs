using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Blazor.Edit
{
    public partial class GridEditCellBoolean
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public object Data { get; set; }

        [Parameter]
        public PropertyInfo PropInfo { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
            InnerValue = (bool?)PropInfo.GetValue(Data) ?? false;
        }

        public void InnerValueOnChange(ChangeEventArgs args)
        {
            PropInfo.SetValue(Data, innerValue);
        }
    }
}
