using SimConnect;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using MSFS2020Companion.Controls;

namespace MSFS2020Companion
{
	class ClientController : ClientControllerBase
	{
		SocketClient simConnect;
		public const float MagicSimConnectNumber = 16383f;

		object trimLock = new object();
		Action nextTrimCall = null;

		public ClientController ()
		{
			simConnect = new SocketClient ("Managed Data Request", "GamerPC2020", 11000);

			foreach (var ctrl in controls)
			{
				ctrl.MapEvent (simConnect);
				ctrl.RegisterDataDefinition (simConnect);
				ctrl.StartDataRequest (simConnect);
			}

			StartLoop ();
		}

		void StartLoop ()
		{
			Task.Run (() =>
			{
				while (true)
				{
					Action action;
					lock (trimLock)
					{
						action = nextTrimCall;
						nextTrimCall = null;
					}
					action?.Invoke ();

					try
					{
						var nextDispatch = simConnect.GetNextDispatch ();
						switch (nextDispatch.Id)
						{
							case RecvId.SimobjectData:
								var data = nextDispatch as RecvSimObjectData;
								var requestId = (MyRequests)data.RequestId;
								foreach (var ctrl in controls)
								{
									if (ctrl.RequestId == requestId)
										Device.InvokeOnMainThreadAsync (() => ctrl.UpdateFromObjectData (data));
								}
								break;
							default:
								break;
						}
					}
					catch (Exception)
					{
						Thread.Sleep (10);
					}
				}
			});
		}

		protected override void SendValueRequest (ControlBase control)
		{
			Action nextCall = () =>
			{
				control.TransmitUpdateRequest (simConnect);
			};

			lock (trimLock)
				nextTrimCall = nextCall;
		}
	}
}
