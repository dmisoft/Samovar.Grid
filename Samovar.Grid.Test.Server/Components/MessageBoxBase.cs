//using Microsoft.AspNetCore.Components;
//using Microsoft.JSInterop;
//using System;
//using System.Threading.Tasks;

//namespace SamovarGridProServerTest.Components
//{
//    public class MessageBoxBase
//        : ComponentBase
//    {
//        [Inject]
//        IJSRuntime JsRuntime { get; set; }

//        public string id = Guid.NewGuid().ToString();
//        [Parameter]
//        public RenderFragment ChildContent { get; set; }
//        [Parameter]
//        public string Text { get; set; }
//        [Parameter]
//        public Action<bool> DialogClosed { get; set; }
//        [CascadingParameter]/*(Name = "datagrid-container")]*/
//        public IDialogablePage DialogablePage { get; set; }

//        public async Task Show()
//        {
//            //this.DialogablePage.DialogRender = this.CreateComponent();
//            await Common.JsInteropClasses.Show(this.id, JsRuntime);
//        }

//        override protected async Task OnAfterRenderAsync(bool firstRender)
//        {
//            if(firstRender)
//                await SamovarGridProServerTest.Common.JsInteropClasses.Show(this.id, JsRuntime);
//        }

//        protected override void OnInitialized()
//        {
//            base.OnInitialized();
//        }
//        //int counter;
//        public RenderFragment Show(string messageText) => builder =>
//        {
//            string key = Guid.NewGuid().ToString();
//            builder.OpenComponent(0,typeof(MessageBox));
//            builder.SetKey(key);

//            builder.AddAttribute(1, "Text", messageText);
//            //builder.AddComponentReferenceCapture(1, inst => { dialog = (ModalDialogComponent)inst; });
//            //builder.AddComponentReferenceCapture(1, DialogCapture);
//            builder.CloseComponent();
//            //builder.OpenComponent(1,typeof(CascadingValue<>))
//        };

//        private void OnDialogClose()
//        {
//            SamovarGridProServerTest.Common.JsInteropClasses.RemoveElementById(this.id, JsRuntime);
//        }

//        protected void OnDialogClosed() {
//            DialogClosed?.Invoke(true);
//        }
//    }
//}
