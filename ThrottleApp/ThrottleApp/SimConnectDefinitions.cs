using System.Runtime.InteropServices;

namespace ThrottleApp
{
	enum MyDefinitions
	{
		TrimValue,
		ThrottleValue,
	}

	enum MyRequests
	{
		TrimValueFetch,
		ThrottleValueFetch,
	}

	enum MyGroups
	{
		Default
	}

	enum MyEvents
	{
		ElevatorTrimSet, //ELEVATOR_TRIM_SET
		ThrottleSet, //THROTTLE_SET
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
}
