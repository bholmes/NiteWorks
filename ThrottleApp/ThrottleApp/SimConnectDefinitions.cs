using System.Runtime.InteropServices;

namespace ThrottleApp
{
	enum MyDefinitions
	{
		TrimValue,
		ThrottleValue,
		LandingGearValue,
		FlapsValue,
		ParkingBrakesValue,
	}

	enum MyRequests
	{
		TrimValueFetch,
		ThrottleValueFetch,
		LandingGearValueFetch,
		FlapsValueFetch,
		ParkingBrakesValueFetch,
	}

	enum MyGroups
	{
		Default
	}

	enum MyEvents
	{
		ElevatorTrimSet, //ELEVATOR_TRIM_SET
		ThrottleSet, //THROTTLE_SET
		GearSet, //GEAR_SET
		FlapsSet, //FLAPS_SET
		ParkingBreaks, //PARKING_BRAKES
	}

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	struct TrimValue
	{
		public float TrimPosition;
	}

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	struct ThrottleValue
	{
		public float GeneralEngThrottleLeverPosition1;
	}

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	struct LandingGearValue
	{
		public int GearHandlePosition;
	}

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	struct FlapsValue
	{
		public int FlapsHandleIndex;
		public int FlapsNumHandlePositions;
	}

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	struct ParkingBrakesValue
	{
		public int BrakeParkingPosition;
	}
}
