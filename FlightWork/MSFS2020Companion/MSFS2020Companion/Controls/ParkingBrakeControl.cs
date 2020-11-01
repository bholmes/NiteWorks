using System;
using SimConnect;

namespace MSFS2020Companion.Controls
{
	class ParkingBrakeControl : ControlBase
	{
		public ParkingBrakeControl () : base (new MainPageViewModel.ModelItem (0), MyRequests.ParkingBrakesValueFetch) { }

		public override void AssignViewModelProperties (MainPageViewModel viewModel)
		{
			viewModel.ParkingBrake = ModelItem;
		}

		public override void MapEvent (SocketClient simConnect)
		{
			simConnect.MapClientEventToSimEvent (MyEvents.ParkingBreaks, "PARKING_BRAKES");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.ParkingBreaks);
		}

		public override void RegisterDataDefinition (SocketClient simConnect)
		{
			simConnect.AddToDataDefinition (MyDefinitions.ParkingBrakesValue, "BRAKE PARKING POSITION", "Bool", Datatype.Int32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.ParkingBrakesValue, typeof (ParkingBrakesValue));
		}

		public override void StartDataRequest (SocketClient simConnect)
		{
			simConnect.RequestDataOnSimObject (MyRequests.ParkingBrakesValueFetch, MyDefinitions.ParkingBrakesValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
		}

		public override void UpdateFromObjectData (RecvSimObjectData data)
		{
			var parkingBrakesValue = (ParkingBrakesValue)data.Data;
			ModelItem.Value = parkingBrakesValue.BrakeParkingPosition;
		}

		public override void TransmitUpdateRequest (SocketClient simConnect)
		{
			unchecked
			{
				if (ModelItem.ValueRequest != ModelItem.Value)
					simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ParkingBreaks, (uint)ModelItem.ValueRequest, MyGroups.Default, EventFlag.Default);
			}
		}
	}
}
