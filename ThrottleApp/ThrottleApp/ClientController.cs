using SimConnect;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThrottleApp
{
	class ClientController : ClientControllerBase
	{
		SocketClient simConnect;
		const float magicSimConnectNumber = 16383f;

		object trimLock = new object();
		Action nextTrimCall = null;

		public ClientController ()
		{
			simConnect = new SocketClient ("Managed Data Request", "GamerPC2020", 11000);

			simConnect.MapClientEventToSimEvent (MyEvents.ElevatorTrimSet, "ELEVATOR_TRIM_SET");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.ElevatorTrimSet);

			simConnect.MapClientEventToSimEvent (MyEvents.ThrottleSet, "THROTTLE_SET");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.ThrottleSet);

			simConnect.AddToDataDefinition (MyDefinitions.TrimValue, "ELEVATOR TRIM INDICATOR", "Position", Datatype.Float32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.TrimValue, typeof (TrimValue));

			simConnect.AddToDataDefinition (MyDefinitions.ThrottleValue, "GENERAL ENG THROTTLE LEVER POSITION:1", "Percent over 100", Datatype.Float32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.ThrottleValue, typeof (ThrottleValue));

			simConnect.RequestDataOnSimObject (MyRequests.TrimValueFetch, MyDefinitions.TrimValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
			simConnect.RequestDataOnSimObject (MyRequests.ThrottleValueFetch, MyDefinitions.ThrottleValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
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
								switch ((MyRequests)data.RequestId)
								{
									case MyRequests.TrimValueFetch:
										var trimValue = (TrimValue)data.Data;
										SetTrimValue (trimValue.TrimPosition);
										break;

									case MyRequests.ThrottleValueFetch:
										var throttleValue = (ThrottleValue)data.Data;
										SetThrottleValue (throttleValue.GeneralEngThrottleLeverPosition1);
										break;
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

		override protected void SendTrimRequest (float trimValueRequest)
		{
			Action nextCall = () =>
			{
				var convertedValue = (int)Math.Round (magicSimConnectNumber * trimValueRequest);
				unchecked
				{
					simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ElevatorTrimSet, (uint)convertedValue, MyGroups.Default, EventFlag.Default);
				}
			};

			lock (trimLock)
				nextTrimCall = nextCall;
		}

		override protected void SendThrottleRequest (float throttleValueRequest)
		{
			Action nextCall = () =>
			{
				var convertedValue = (int)Math.Round (magicSimConnectNumber * throttleValueRequest);
				unchecked
				{
					simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ThrottleSet, (uint)convertedValue, MyGroups.Default, EventFlag.Default);
				}
			};

			lock (trimLock)
				nextTrimCall = nextCall;
		}
	}
}
