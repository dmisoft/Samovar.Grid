using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Grid.Edit
{
    public partial class GridEditCellBoolean
    {
        [Parameter]
        public required object Data { get; set; }

        [Parameter]
        public required PropertyInfo PropInfo { get; set; }

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
