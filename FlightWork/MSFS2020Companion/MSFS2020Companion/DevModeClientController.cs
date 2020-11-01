using System.Threading;
using Xamarin.Forms;
using MSFS2020Companion.Controls;
using System.Threading.Tasks;

namespace MSFS2020Companion
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
