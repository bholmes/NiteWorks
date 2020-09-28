using System.Threading;
using Xamarin.Forms;
using ThrottleApp.Controls;
using System.Threading.Tasks;

namespace ThrottleApp
{
	class DevModeClientController : ClientControllerBase
	{
		protected override void SendValueRequest (ControlBase control)
		{
			Task.Run (() =>
			{
				Thread.Sleep (10);
				Device.InvokeOnMainThreadAsync (() => control.ModelItem.Value = control.ModelItem.ValueRequest);
			});
		}
	}
}
