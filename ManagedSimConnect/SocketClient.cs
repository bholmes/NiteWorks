using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SimConnect
{
	public class SocketClient : IDisposable
	{
		bool disposedValue;
		Socket socket;
		Dictionary<uint, Type> dataDefinitionTypeLookup = new Dictionary<uint, Type> ();

		public SocketClient (string name, string serverHostName, int port)
		{
			ConnectToServer (serverHostName, port);
			var hr = InvokeOverSocket (new MethodCall.Open (name)).Hresult;
			if (hr < 0)
				throw new Exception ($"Server did not connec with hr = {hr}");
		}

		public void Close ()
		{
			Dispose ();
		}

		private void EnsureConnected ()
		{
			if (socket == null || !socket.Connected)
				throw new Exception ("Not Connected");
		}

		MethodCall.ReturnValue InvokeOverSocket (MethodCall methodCall)
		{
			EnsureConnected ();
			socket.Send (methodCall.ToByteArray ());
			byte [] iBuff = new byte [1024];
			socket.Receive (iBuff);
			return MethodCall.ReturnValue.FromByteArray (iBuff);
		}

		public void GetNextDispatch (byte [] buff, out uint cbData)
		{
			var ret = (MethodCall.ReturnValue.GetNextDispatch)InvokeOverSocket (new MethodCall.GetNextDispatch ());

			cbData = ret.CbData;
			if (cbData > 0)
			{
				if (ret.Data == null)
					throw new Exception ("Expected data but received null.");

				ret.Data.CopyTo (buff, 0);
			}

			CheckHresult (ret.Hresult);
		}

		unsafe public Recv GetNextDispatch ()
		{
			var ret = (MethodCall.ReturnValue.GetNextDispatch)InvokeOverSocket (new MethodCall.GetNextDispatch ());
			CheckHresult (ret.Hresult);

			fixed (byte* b = &ret.Data [0])
			{
				var pData = (IntPtr)b;

				var pRecv = Marshal.PtrToStructure<SIMCONNECT_RECV> (pData);
				switch ((RecvId)pRecv.dwID)
				{
					case RecvId.Event:
						return RecvEvent.FromMemory (pData);

					case RecvId.SimobjectData:
						return RecvSimObjectData.FromMemory (pData, dataDefinitionTypeLookup);

					case RecvId.SimobjectDataByType:
						return RecvSimObjectDataByType.FromMemory (pData, dataDefinitionTypeLookup);

					default:
						return Recv.FromMemory (pData);
				}
			}
		}

		void CheckHresult (int hr, [CallerMemberName] string callerName = "")
		{
			if (hr < 0)
				throw new Exception ($"Error code from {callerName} : {hr}");
		}

		public void MapClientEventToSimEvent (Enum eventId, string eventName = "")
		{
			CheckHresult (InvokeOverSocket (new MethodCall.MapClientEventToSimEvent (Convert.ToUInt32 (eventId), eventName)).Hresult);
		}

		public void AddClientEventToNotificationGroup (Enum groupId, Enum eventId, bool bMaskable = false)
		{
			CheckHresult (InvokeOverSocket (new MethodCall.AddClientEventToNotificationGroup (Convert.ToUInt32 (groupId), Convert.ToUInt32 (eventId), bMaskable ? 1 : 0)).Hresult);
		}

		public void SetNotificationGroupPriority (Enum groupId, uint priority)
		{
			CheckHresult (InvokeOverSocket (new MethodCall.SetNotificationGroupPriority (Convert.ToUInt32 (groupId), priority)).Hresult);
		}

		public void AddToDataDefinition (Enum defineId, string datumName, string unitsName, Datatype datumType = Datatype.Float64, float fEpsilon = 0, uint datumId = Constants.Unused)
		{
			CheckHresult (InvokeOverSocket (new MethodCall.AddToDataDefinition (Convert.ToUInt32 (defineId), datumName, unitsName, datumType, fEpsilon, datumId)).Hresult);
		}

		public void RegisterStructToDataDefinition (Enum defineId, Type type)
		{
			dataDefinitionTypeLookup.Add (Convert.ToUInt32 (defineId), type);
		}

		public void RequestDataOnSimObject (Enum requestId, Enum defineId, uint objectId, Period period, DataRequestFlag flags = 0, uint origin = 0, uint interval = 0, uint limit = 0)
		{
			CheckHresult (InvokeOverSocket (new MethodCall.RequestDataOnSimObject (Convert.ToUInt32 (requestId), Convert.ToUInt32 (defineId), objectId, period, flags, origin, interval, limit)).Hresult);
		}

		public void TransmitClientEvent (uint objectId, Enum eventId, uint dwData, Enum groupId, EventFlag flags)
		{
			CheckHresult (InvokeOverSocket (new MethodCall.TransmitClientEvent (objectId, Convert.ToUInt32 (eventId), dwData, Convert.ToUInt32 (groupId), flags)).Hresult);
		}

		void ConnectToServer (string serverHostName, int port)
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry (serverHostName);
			IPAddress ipAddress = ipHostInfo.AddressList [0];
			IPEndPoint remoteEP = new IPEndPoint (ipAddress, port);

			socket = new Socket (ipAddress.AddressFamily,
					 SocketType.Stream, ProtocolType.Tcp);

			socket.Connect (remoteEP);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					socket.Shutdown (SocketShutdown.Both);
					socket.Close ();
					socket.Dispose ();
					socket = null;
				}

				disposedValue = true;
			}
		}

		~SocketClient ()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose (disposing: false);
		}

		public void Dispose ()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose (disposing: true);
			GC.SuppressFinalize (this);
		}
	}
}
