using System;
using SimConnect;

namespace MSFS2020Companion.Controls
{
	class FlapsControl : ControlBase
	{
		public FlapsControl () : base (new MainPageViewModel.ModelItem (0), MyRequests.FlapsValueFetch) { }

		public override void AssignViewModelProperties (MainPageViewModel viewModel)
		{
			viewModel.FlapsHandleIndex = ModelItem;
			viewModel.FlapsNumHandlePositions = NumHandlePositionsModelItem;
		}

		public override void MapEvent (SocketClient simConnect)
		{
			simConnect.MapClientEventToSimEvent (MyEvents.FlapsSet, "FLAPS_SET");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.FlapsSet);
		}

		public override void RegisterDataDefinition (SocketClient simConnect)
		{
			simConnect.AddToDataDefinition (MyDefinitions.FlapsValue, "FLAPS HANDLE INDEX", "Number", Datatype.Int32, 0f, Constants.Unused);
			simConnect.AddToDataDefinition (MyDefinitions.FlapsValue, "FLAPS NUM HANDLE POSITIONS", "Number", Datatype.Int32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.FlapsValue, typeof (FlapsValue));
		}

		public override void StartDataRequest (SocketClient simConnect)
		{
			simConnect.RequestDataOnSimObject (MyRequests.FlapsValueFetch, MyDefinitions.FlapsValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
		}

		public override void UpdateFromObjectData (RecvSimObjectData data)
		{
			var flapsValue = (FlapsValue)data.Data;
			NumHandlePositionsModelItem.Value = flapsValue.FlapsNumHandlePositions;
			ModelItem.Value = (flapsValue.FlapsHandleIndex);
		}

		public override void TransmitUpdateRequest (SocketClient simConnect)
		{
			unchecked
			{
				var adjustedValue = (uint)Math.Round (ClientController.MagicSimConnectNumber * (float)ModelItem.ValueRequest / (float)NumHandlePositionsModelItem.Value);
				simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.FlapsSet, adjustedValue, MyGroups.Default, EventFlag.Default);
			}
		}

		public MainPageViewModel.ModelItem NumHandlePositionsModelItem { get; } = new MainPageViewModel.ModelItem (3);
	}
}
