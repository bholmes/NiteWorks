using System;
using SimConnect;

namespace MSFS2020Companion.Controls
{
	class TrimControl : ControlBase
	{
		public TrimControl () : base (new MainPageViewModel.ModelItem (0), MyRequests.TrimValueFetch) { }

		public override void AssignViewModelProperties (MainPageViewModel viewModel)
		{
			viewModel.Trim = ModelItem;
		}

		public override void MapEvent (SocketClient simConnect)
		{
			simConnect.MapClientEventToSimEvent (MyEvents.ElevatorTrimSet, "ELEVATOR_TRIM_SET");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.ElevatorTrimSet);
		}

		public override void RegisterDataDefinition (SocketClient simConnect)
		{
			simConnect.AddToDataDefinition (MyDefinitions.TrimValue, "ELEVATOR TRIM INDICATOR", "Position", Datatype.Float32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.TrimValue, typeof (TrimValue));
		}

		public override void StartDataRequest (SocketClient simConnect)
		{
			simConnect.RequestDataOnSimObject (MyRequests.TrimValueFetch, MyDefinitions.TrimValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
		}

		public override void UpdateFromObjectData (RecvSimObjectData data)
		{
			var trimValue = (TrimValue)data.Data;
			ModelItem.Value = (int)Math.Round (trimValue.TrimPosition * ClientController.MagicSimConnectNumber);
		}

		public override void TransmitUpdateRequest (SocketClient simConnect)
		{
			unchecked
			{
				simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ElevatorTrimSet, (uint)ModelItem.ValueRequest, MyGroups.Default, EventFlag.Default);
			}
		}
	}
}
