using System;
using SimConnect;

namespace MSFS2020Companion.Controls
{
	class LandingGearControl : ControlBase
	{
		public LandingGearControl () : base (new MainPageViewModel.ModelItem (0), MyRequests.LandingGearValueFetch) { }

		public override void AssignViewModelProperties (MainPageViewModel viewModel)
		{
			viewModel.LandingGear = ModelItem;
		}

		public override void MapEvent (SocketClient simConnect)
		{
			simConnect.MapClientEventToSimEvent (MyEvents.GearSet, "GEAR_SET");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.GearSet);
		}

		public override void RegisterDataDefinition (SocketClient simConnect)
		{
			simConnect.AddToDataDefinition (MyDefinitions.LandingGearValue, "GEAR HANDLE POSITION", "Bool", Datatype.Int32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.LandingGearValue, typeof (LandingGearValue));
		}

		public override void StartDataRequest (SocketClient simConnect)
		{
			simConnect.RequestDataOnSimObject (MyRequests.LandingGearValueFetch, MyDefinitions.LandingGearValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
		}

		public override void UpdateFromObjectData (RecvSimObjectData data)
		{
			var landingGearValue = (LandingGearValue)data.Data;
			ModelItem.Value = landingGearValue.GearHandlePosition;
		}

		public override void TransmitUpdateRequest (SocketClient simConnect)
		{
			unchecked
			{
				simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.GearSet, (uint)ModelItem.ValueRequest, MyGroups.Default, EventFlag.Default);
			}
		}
	}
}
