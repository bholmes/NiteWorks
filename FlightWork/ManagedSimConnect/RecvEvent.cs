using System;
using System.Runtime.InteropServices;

namespace SimConnect
{
	public class RecvEvent : Recv
	{
		internal RecvEvent (uint dwSize, uint dwVersion, uint dwID, uint uGroupID, uint uEventID, uint dwData) : base (dwSize, dwVersion, dwID)
		{
			GroupId = uGroupID;
			EventId = uEventID;
			Data = dwData;
		}

		internal RecvEvent (SIMCONNECT_RECV_EVENT sRecvEvent) : this (sRecvEvent.dwSize, sRecvEvent.dwVersion, sRecvEvent.dwID, sRecvEvent.uGroupID, sRecvEvent.uEventID, sRecvEvent.dwData) { }

		internal new static RecvEvent FromMemory (IntPtr pData)
		{
			return new RecvEvent (Marshal.PtrToStructure<SIMCONNECT_RECV_EVENT> (pData));
		}

		public uint GroupId { get; private set; }
		public uint EventId { get; private set; }
		public uint Data { get; private set; }
	}
}
