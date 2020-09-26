using System.Threading;
using Xamarin.Forms;
using ThrottleApp.Controls;

namespace ThrottleApp
{
	class DevModeClientController : ClientControllerBase
	{
		protected override void SendValueRequest (ControlBase control)
		{
			Thread.Sleep (10);
			Device.InvokeOnMainThreadAsync (() => control.ModelItem.Value = control.ModelItem.ValueRequest);
		}
	}
}
