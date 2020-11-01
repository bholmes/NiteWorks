using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SimConnect
{
	public class RecvSimObjectData : Recv
	{
		internal RecvSimObjectData (uint dwSize, uint dwVersion, uint dwID, uint dwRequestID, uint dwObjectID, uint dwDefineID, uint dwFlags, uint dwentrynumber, uint dwoutof, uint dwDefineCount) : base (dwSize, dwVersion, dwID)
		{
			RequestId = dwRequestID;
			ObjectId = dwObjectID;
			DefineId = dwDefineID;
			Flags = dwFlags;
			EntryNumber = dwentrynumber;
			OutOf = dwoutof;
			DefineCount = dwDefineCount;
		}

		internal RecvSimObjectData (SIMCONNECT_RECV_SIMOBJECT_DATA sRecvSimObjectData) : this (sRecvSimObjectData.dwSize, sRecvSimObjectData.dwVersion, sRecvSimObjectData.dwID, sRecvSimObjectData.dwRequestID, sRecvSimObjectData.dwObjectID, sRecvSimObjectData.dwDefineID, sRecvSimObjectData.dwFlags, sRecvSimObjectData.dwentrynumber, sRecvSimObjectData.dwoutof, sRecvSimObjectData.dwDefineCount) { }

		internal static RecvSimObjectData FromMemory (IntPtr pData, Dictionary<uint, Type> dataDefinitionTypeLookup)
		{
			var ret = new RecvSimObjectData (Marshal.PtrToStructure<SIMCONNECT_RECV_SIMOBJECT_DATA> (pData));
			var sType = dataDefinitionTypeLookup [ret.DefineId];
			ret.Data = Marshal.PtrToStructure (pData + (SIMCONNECT_RECV_SIMOBJECT_DATA.dwDataStart), sType);
			return ret;
		}

		public uint RequestId { get; private set; }
		public uint ObjectId { get; private set; }
		public uint DefineId { get; private set; }
		public uint Flags { get; private set; }
		public uint EntryNumber { get; private set; }
		public uint OutOf { get; private set; }
		public uint DefineCount { get; private set; }
		public object Data { get; protected set; }
	}
}
