using System;
using System.Runtime.InteropServices;

using HRESULT = System.Int32;
using HANDLE = System.IntPtr;
using LPCSTR = System.String;
using HWND = System.IntPtr;
using DWORD = System.UInt32;

namespace SimConnect
{
    internal class SimConnectNative
	{
		const CallingConvention SinConnectCallingConvention = CallingConvention.StdCall;

		[DllImport(dllName: "SimConnect.dll", EntryPoint = "SimConnect_Open",  CallingConvention = SinConnectCallingConvention)]
		internal static extern HRESULT Open(out HANDLE phSimConnect, [MarshalAs(UnmanagedType.LPStr)] LPCSTR szName, HWND hWnd, DWORD UserEventWin32, HANDLE hEventHandle, DWORD ConfigIndex);

		[DllImport(dllName: "SimConnect.dll", EntryPoint= "SimConnect_Close", CallingConvention = SinConnectCallingConvention)]
		internal static extern HRESULT Close(HANDLE hSimConnect);

		[DllImport(dllName: "SimConnect.dll", EntryPoint = "SimConnect_GetNextDispatch", CallingConvention = SinConnectCallingConvention)]
		internal static extern HRESULT GetNextDispatch(HANDLE hSimConnect, ref IntPtr ppData, ref DWORD pcbData);

		[DllImport(dllName: "SimConnect.dll", EntryPoint = "SimConnect_MapClientEventToSimEvent", CallingConvention = SinConnectCallingConvention)]
		internal static extern HRESULT MapClientEventToSimEvent(HANDLE hSimConnect, DWORD EventID, [MarshalAs(UnmanagedType.LPStr)] string EventName = "");

		[DllImport(dllName: "SimConnect.dll", EntryPoint = "SimConnect_AddClientEventToNotificationGroup", CallingConvention = SinConnectCallingConvention)]
		internal static extern HRESULT AddClientEventToNotificationGroup(HANDLE hSimConnect, DWORD GroupID, DWORD EventID, int bMaskable = 0);

		[DllImport(dllName: "SimConnect.dll", EntryPoint = "SimConnect_SetNotificationGroupPriority", CallingConvention = SinConnectCallingConvention)]
		internal static extern HRESULT SetNotificationGroupPriority(HANDLE hSimConnect, DWORD GroupID, DWORD uPriority);

		[DllImport (dllName: "SimConnect.dll", EntryPoint = "SimConnect_AddToDataDefinition", CallingConvention = SinConnectCallingConvention)]
		internal static extern HRESULT AddToDataDefinition (HANDLE hSimConnect, DWORD DefineID, [MarshalAs (UnmanagedType.LPStr)] string DatumName, [MarshalAs (UnmanagedType.LPStr)] string UnitsName, Datatype DatumType = Datatype.Float64, float fEpsilon = 0, DWORD DatumID = Constants.Unused);

		[DllImport(dllName: "SimConnect.dll", EntryPoint = "SimConnect_RequestDataOnSimObjectType", CallingConvention = SinConnectCallingConvention)]
		internal static extern HRESULT RequestDataOnSimObjectType(HANDLE hSimConnect, DWORD RequestID, DWORD DefineID, DWORD dwRadiusMeters, SimObjectType type);

		[DllImport(dllName: "SimConnect.dll", EntryPoint = "SimConnect_TransmitClientEvent", CallingConvention = SinConnectCallingConvention)]
		internal static extern HRESULT TransmitClientEvent(HANDLE hSimConnect, DWORD ObjectID, DWORD EventID, DWORD dwData, DWORD GroupID, EventFlag Flags);

		[DllImport(dllName: "SimConnect.dll", EntryPoint = "SimConnect_RequestDataOnSimObject", CallingConvention = SinConnectCallingConvention)]
		internal static extern HRESULT RequestDataOnSimObject(HANDLE hSimConnect, DWORD RequestID, DWORD DefineID, DWORD ObjectID, Period Period, DataRequestFlag Flags = 0, DWORD origin = 0, DWORD interval = 0, DWORD limit = 0);
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SIMCONNECT_RECV
	{
		public DWORD dwSize;         // record size
		public DWORD dwVersion;      // interface version
		public DWORD dwID;           // see SIMCONNECT_RECV_ID
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SIMCONNECT_RECV_EVENT // : SIMCONNECT_RECV       // when dwID == SIMCONNECT_RECV_ID_EVENT
	{
		// SIMCONNECT_RECV fields
		public DWORD dwSize;         // record size
		public DWORD dwVersion;      // interface version
		public DWORD dwID;           // see SIMCONNECT_RECV_ID

		// SIMCONNECT_RECV_EVENT fields
		//static const DWORD UNKNOWN_GROUP = DWORD_MAX;
		public DWORD uGroupID;
		public DWORD uEventID;
		public DWORD dwData;       // uEventID-dependent context
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SIMCONNECT_RECV_SIMOBJECT_DATA // : public SIMCONNECT_RECV           // when dwID == SIMCONNECT_RECV_ID_SIMOBJECT_DATA
	{
		// SIMCONNECT_RECV fields
		public DWORD dwSize;         // record size
		public DWORD dwVersion;      // interface version
		public DWORD dwID;           // see SIMCONNECT_RECV_ID

		// SIMCONNECT_RECV_SIMOBJECT_DATA
		public DWORD dwRequestID;
		public DWORD dwObjectID;
		public DWORD dwDefineID;
		public DWORD dwFlags;            // SIMCONNECT_DATA_REQUEST_FLAG
		public DWORD dwentrynumber;      // if multiple objects returned, this is number <entrynumber> out of <outof>.
		public DWORD dwoutof;            // note: starts with 1, not 0.          
		public DWORD dwDefineCount;      // data count (number of datums, *not* byte count)

		//SIMCONNECT_DATAV(dwData, dwDefineID, );             // data begins here, dwDefineCount data items
		public const int dwDataStart = sizeof(DWORD) * 10;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE // : public SIMCONNECT_RECV           // when dwID == SIMCONNECT_RECV_ID_SIMOBJECT_DATA
	{
		// SIMCONNECT_RECV fields
		public DWORD dwSize;         // record size
		public DWORD dwVersion;      // interface version
		public DWORD dwID;           // see SIMCONNECT_RECV_ID

		// SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE
		public DWORD dwRequestID;
		public DWORD dwObjectID;
		public DWORD dwDefineID;
		public DWORD dwFlags;            // SIMCONNECT_DATA_REQUEST_FLAG
		public DWORD dwentrynumber;      // if multiple objects returned, this is number <entrynumber> out of <outof>.
		public DWORD dwoutof;            // note: starts with 1, not 0.          
		public DWORD dwDefineCount;      // data count (number of datums, *not* byte count)

		//SIMCONNECT_DATAV(dwData, dwDefineID, );             // data begins here, dwDefineCount data items
		public const int dwDataStart = sizeof(DWORD) * 10;
	}


	public enum RecvId
	{
		Null,
		Exception,
		Open,
		Quit,
		Event,
		Event_object_addremove,
		EventFilename,
		EventFrame,
		SimobjectData,
		SimobjectDataByType,
		WeatherObservation,
		CloudState,
		AssignedObjectId,
		ReservedKey,
		CustomAction,
		SystemState,
		ClientData,
		EventWeatherMode,
		AirportList,
		Vor_list, RServerStarted,
		EventMultiplayerClientStarted,
		EventMultiplayerSessionEnded,
		EventRaceEnd,
		EventRaceKLap,
	}

	public enum Period
	{
		Never,
		Once,
		VisualFrame,
		SimFrame,
		Second,
	}

	public enum Datatype
	{
		Invalid,        // invalid data type
		Int32,          // 32-bit integer number
		Int64,          // 64-bit integer number
		Float32,        // 32-bit floating-point number (float)
		Float64,        // 64-bit floating-point number (double)
		String8,        // 8-byte string
		String32,       // 32-byte string
		String64,       // 64-byte string
		String128,      // 128-byte string
		String256,      // 256-byte string
		String260,      // 260-byte string
		Stringv,        // variable-length string

		Initposition,   // see SIMCONNECT_DATA_INITPOSITION
		Markerstate,    // see SIMCONNECT_DATA_MARKERSTATE
		Eaypoint,       // see SIMCONNECT_DATA_WAYPOINT
		Latlonalt,      // see SIMCONNECT_DATA_LATLONALT
		XYZ,            // see SIMCONNECT_DATA_XYZ

		Max             // enum limit
	}

	[Flags]
	public enum EventFlag
	{
		Default = 0x00000000,
		FastRepeatTimer = 0x00000001,      // set event repeat timer to simulate fast repeat
		SlowRepeatTimer = 0x00000002,      // set event repeat timer to simulate slow repeat
		GroupidIsPriority = 0x00000010,      // interpret GroupID parameter as priority value
	}

	[Flags]
	public enum DataRequestFlag
	{
		Default = 0x00000000,
		Changed = 0x00000001,      // send requested data when value(s) change
		Tagged = 0x00000002,      // send requested data in tagged format
	}

	public enum SimObjectType
	{
		User = 0,
		All = 1,
		Aircraft = 2,
		Helicopter = 3,
		Boat = 4,
		Ground = 5
	}

	public class Constants
	{
		private Constants () { }
		public const DWORD Unused = UInt32.MaxValue;
		public const DWORD ObjectIdUser = 0;
	}

	public class GroupPriority
	{
		private GroupPriority () { }

		// Notification Group priority values
		public const DWORD Highest = 1;      // highest priority
		public const DWORD HighestMaskable = 10000000;      // highest priority that allows events to be masked
		public const DWORD Standard = 1900000000;      // standard priority
		public const DWORD Default = 2000000000;      // default priority
		public const DWORD Lowest = 4000000000;      // priorities lower than this will be ignored
	}
}
