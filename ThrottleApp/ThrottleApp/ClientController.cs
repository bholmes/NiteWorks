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

			simConnect.MapClientEventToSimEvent (MyEvents.GearSet, "GEAR_SET");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.GearSet);

			simConnect.MapClientEventToSimEvent (MyEvents.FlapsSet, "FLAPS_SET");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.FlapsSet);

			simConnect.MapClientEventToSimEvent (MyEvents.ParkingBreaks, "PARKING_BRAKES");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.ParkingBreaks);

			simConnect.AddToDataDefinition (MyDefinitions.TrimValue, "ELEVATOR TRIM INDICATOR", "Position", Datatype.Float32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.TrimValue, typeof (TrimValue));

			simConnect.AddToDataDefinition (MyDefinitions.ThrottleValue, "GENERAL ENG THROTTLE LEVER POSITION:1", "Percent over 100", Datatype.Float32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.ThrottleValue, typeof (ThrottleValue));

			simConnect.AddToDataDefinition (MyDefinitions.LandingGearValue, "GEAR HANDLE POSITION", "Bool", Datatype.Int32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.LandingGearValue, typeof (LandingGearValue));

			simConnect.AddToDataDefinition (MyDefinitions.FlapsValue, "FLAPS HANDLE INDEX", "Number", Datatype.Int32, 0f, Constants.Unused);
			simConnect.AddToDataDefinition (MyDefinitions.FlapsValue, "FLAPS NUM HANDLE POSITIONS", "Number", Datatype.Int32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.FlapsValue, typeof (FlapsValue));

			simConnect.AddToDataDefinition (MyDefinitions.ParkingBrakesValue, "BRAKE PARKING POSITION", "Bool", Datatype.Int32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.ParkingBrakesValue, typeof (ParkingBrakesValue));

			simConnect.RequestDataOnSimObject (MyRequests.TrimValueFetch, MyDefinitions.TrimValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
			simConnect.RequestDataOnSimObject (MyRequests.ThrottleValueFetch, MyDefinitions.ThrottleValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
			simConnect.RequestDataOnSimObject (MyRequests.LandingGearValueFetch, MyDefinitions.LandingGearValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
			simConnect.RequestDataOnSimObject (MyRequests.FlapsValueFetch, MyDefinitions.FlapsValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
			simConnect.RequestDataOnSimObject (MyRequests.ParkingBrakesValueFetch, MyDefinitions.ParkingBrakesValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
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
										SetTrimValue ((int)Math.Round (trimValue.TrimPosition * magicSimConnectNumber));
										break;

									case MyRequests.ThrottleValueFetch:
										var throttleValue = (ThrottleValue)data.Data;
										SetThrottleValue ((int)Math.Round (throttleValue.GeneralEngThrottleLeverPosition1 * magicSimConnectNumber));
										break;
									case MyRequests.LandingGearValueFetch:
										var landingGearValue = (LandingGearValue)data.Data;
										SetLandingGearValue (landingGearValue.GearHandlePosition);
										break;
									case MyRequests.FlapsValueFetch:
										var flapsValue = (FlapsValue)data.Data;
										SetFlapsNumHandlePositions (flapsValue.FlapsNumHandlePositions);
										SetFlapsHandleIndex (flapsValue.FlapsHandleIndex);
										break;
									case MyRequests.ParkingBrakesValueFetch:
										var parkingBrakesValue = (ParkingBrakesValue)data.Data;
										SetParkingBrakeValue (parkingBrakesValue.BrakeParkingPosition);
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

		override protected void SendTrimRequest (int trimValueRequest)
		{
			Action nextCall = () =>
			{
				unchecked
				{
					simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ElevatorTrimSet, (uint)trimValueRequest, MyGroups.Default, EventFlag.Default);
				}
			};

			lock (trimLock)
				nextTrimCall = nextCall;
		}

		override protected void SendThrottleRequest (int throttleValueRequest)
		{
			Action nextCall = () =>
			{
				unchecked
				{
					simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ThrottleSet, (uint)throttleValueRequest, MyGroups.Default, EventFlag.Default);
				}
			};

			lock (trimLock)
				nextTrimCall = nextCall;
		}

		override protected void SendLandingGearRequest (int landingGearValueRequest)
		{
			Action nextCall = () =>
			{
				unchecked
				{
					simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.GearSet, (uint)landingGearValueRequest, MyGroups.Default, EventFlag.Default);
				}
			};

			lock (trimLock)
				nextTrimCall = nextCall;
		}

		protected override void SetFlapsHandleIndexRequest (int flapsHandleIndexRequest)
		{
			Action nextCall = () =>
			{
				unchecked
				{
					var adjustedValue = (uint)Math.Round (magicSimConnectNumber * (float)flapsHandleIndexRequest / (float)viewModel.FlapsNumHandlePositions);
					simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.FlapsSet, adjustedValue, MyGroups.Default, EventFlag.Default);
				}
			};

			lock (trimLock)
				nextTrimCall = nextCall;
		}

		protected override void SetParkingBrakeRequest (int parkingBrakeValueRequest)
		{
			Action nextCall = () =>
			{
				unchecked
				{
					if (viewModel.ParkingBrakeValue != parkingBrakeValueRequest)
						simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ParkingBreaks, (uint)parkingBrakeValueRequest, MyGroups.Default, EventFlag.Default);
				}
			};

			lock (trimLock)
				nextTrimCall = nextCall;
		}
	}
}
