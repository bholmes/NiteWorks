using System;
using SimConnect;

namespace ThrottleApp.Controls
{
	class ThrottleControl : ControlBase
	{
		public ThrottleControl () : base (new MainPageViewModel.ModelItem (0), MyRequests.ThrottleValueFetch) { }

		public override void AssignViewModelProperties (MainPageViewModel viewModel)
		{
			viewModel.Throttle = ModelItem;
		}

		public override void MapEvent (SocketClient simConnect)
		{
			simConnect.MapClientEventToSimEvent (MyEvents.ThrottleSet, "THROTTLE_SET");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.ThrottleSet);
		}

		public override void RegisterDataDefinition (SocketClient simConnect)
		{
			simConnect.AddToDataDefinition (MyDefinitions.ThrottleValue, "GENERAL ENG THROTTLE LEVER POSITION:1", "Percent over 100", Datatype.Float32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.ThrottleValue, typeof (ThrottleValue));
		}

		public override void StartDataRequest (SocketClient simConnect)
		{
			simConnect.RequestDataOnSimObject (MyRequests.ThrottleValueFetch, MyDefinitions.ThrottleValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
		}

		public override void UpdateFromObjectData (RecvSimObjectData data)
		{
			var throttleValue = (ThrottleValue)data.Data;
			ModelItem.Value = (int)Math.Round (throttleValue.GeneralEngThrottleLeverPosition1 * ClientController.MagicSimConnectNumber);
		}

		public override void TransmitUpdateRequest (SocketClient simConnect)
		{
			unchecked
			{
				simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ThrottleSet, (uint)ModelItem.ValueRequest, MyGroups.Default, EventFlag.Default);
			}
		}
	}
}
