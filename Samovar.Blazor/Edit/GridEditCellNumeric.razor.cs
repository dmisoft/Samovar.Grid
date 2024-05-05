using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Blazor.Edit
{
    public partial class GridEditCellNumeric<TValue, TEntity>
    {
        [Parameter]
        public required TEntity Data { get; set; }

        [Parameter]
        public required PropertyInfo PropInfo { get; set; }

        private TValue? innerValue;
        protected TValue? InnerValue
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
            InnerValue = (TValue?)PropInfo.GetValue(Data);
        }

        public void InnerValueOnChange(ChangeEventArgs args)
        {
            PropInfo.SetValue(Data, innerValue);
        }
    }
}
