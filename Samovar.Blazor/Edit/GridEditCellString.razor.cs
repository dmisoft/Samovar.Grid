using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Blazor.Edit
{
    public partial class GridEditCellString
    {
        [Parameter]
        public object Data { get; set; }

        [Parameter]
        public PropertyInfo PropInfo { get; set; }

        private string innerValue;
        protected string InnerValue
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
            InnerValue = PropInfo.GetValue(Data)?.ToString();
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
