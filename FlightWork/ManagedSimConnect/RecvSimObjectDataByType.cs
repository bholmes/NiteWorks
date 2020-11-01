using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SimConnect
{
	public class RecvSimObjectDataByType : RecvSimObjectData
	{
		internal RecvSimObjectDataByType (uint dwSize, uint dwVersion, uint dwID, uint dwRequestID, uint dwObjectID, uint dwDefineID, uint dwFlags, uint dwentrynumber, uint dwoutof, uint dwDefineCount) :
			base (dwSize, dwVersion, dwID, dwRequestID, dwObjectID, dwDefineID, dwFlags, dwentrynumber, dwoutof, dwDefineCount)
		{

		}

		internal RecvSimObjectDataByType (SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE sRecvSimObjectDataByType) : this (sRecvSimObjectDataByType.dwSize, sRecvSimObjectDataByType.dwVersion, sRecvSimObjectDataByType.dwID, sRecvSimObjectDataByType.dwRequestID, sRecvSimObjectDataByType.dwObjectID, sRecvSimObjectDataByType.dwDefineID, sRecvSimObjectDataByType.dwFlags, sRecvSimObjectDataByType.dwentrynumber, sRecvSimObjectDataByType.dwoutof, sRecvSimObjectDataByType.dwDefineCount) { }

		internal static new RecvSimObjectDataByType FromMemory (IntPtr pData, Dictionary<uint, Type> dataDefinitionTypeLookup)
		{
			var ret = new RecvSimObjectDataByType (Marshal.PtrToStructure<SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE> (pData));
			var sType = dataDefinitionTypeLookup [ret.DefineId];
			ret.Data = Marshal.PtrToStructure (pData + (SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE.dwDataStart), sType);
			return ret;
		}
	}
}
