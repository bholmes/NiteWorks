using System;
using System.Runtime.InteropServices;

namespace SimConnect
{
	public class Recv
	{
		internal Recv (uint dwSize, uint dwVersion, uint dwID)
		{
			Size = dwSize;
			Version = dwVersion;
			Id = (RecvId)dwID;
		}

		internal Recv (SIMCONNECT_RECV pRecv) : this (pRecv.dwSize, pRecv.dwVersion, pRecv.dwID) { }

		internal static Recv FromMemory (IntPtr pData)
		{
			return new Recv (Marshal.PtrToStructure<SIMCONNECT_RECV> (pData));
		}

		/// <summary>
		/// Record size
		/// </summary>
		public uint Size { get; private set; }

		/// <summary>
		/// interface version
		/// </summary>
		public uint Version { get; private set; }

		/// <summary>
		/// see SIMCONNECT_RECV_ID
		/// </summary>
		public RecvId Id { get; private set; }
	}
}
