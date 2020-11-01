using System;
using System.Runtime.InteropServices;

namespace SimConnect
{
	[Serializable]

	abstract class MethodCall
	{
		public ReturnValue Invoke ()
		{
			return InternalInvoke (IntPtr.Zero);
		}

		public ReturnValue Invoke (IntPtr hSimConnect)
		{
			if (hSimConnect == IntPtr.Zero)
				throw new Exception ("hSimConnect is null");

			return InternalInvoke (hSimConnect);
		}

		protected abstract ReturnValue InternalInvoke (IntPtr hSimConnect);

		internal byte [] ToByteArray ()
		{
			return Serialize.ToByteArray (this);
		}

		static internal MethodCall FromByteArray (byte [] arrBytes)
		{
			return Serialize.FromByteArray<MethodCall> (arrBytes);
		}

		[Serializable]
		internal class Open : MethodCall
		{
			private readonly string szName;

			public Open (string szName)
			{
				this.szName = szName;
			}

			protected override ReturnValue InternalInvoke (IntPtr hSimConnect)
			{
				// hSimConnect should be zero for when calling open.'
				IntPtr retHSimConnect;
				int hr = SimConnectNative.Open (out retHSimConnect, szName, IntPtr.Zero, 0, IntPtr.Zero, 0);

				return new ReturnValue.Open (hr, retHSimConnect);
			}
		}


		[Serializable]
		internal class MapClientEventToSimEvent : MethodCall
		{
			uint eventId;
			string eventName;

			public MapClientEventToSimEvent (uint eventId, string eventName)
			{
				this.eventId = eventId;
				this.eventName = eventName;
			}

			protected override ReturnValue InternalInvoke (IntPtr hSimConnect)
			{
				return new ReturnValue (SimConnectNative.MapClientEventToSimEvent (hSimConnect, Convert.ToUInt32 (eventId), eventName));
			}
		}

		[Serializable]
		internal class AddClientEventToNotificationGroup : MethodCall
		{
			private readonly uint groupId;
			private readonly uint eventId;
			private readonly int bMaskable;

			public AddClientEventToNotificationGroup (uint groupId, uint eventId, int bMaskable)
			{
				this.eventId = eventId;
				this.bMaskable = bMaskable;
				this.groupId = groupId;
			}

			protected override ReturnValue InternalInvoke (IntPtr hSimConnect)
			{
				return new ReturnValue (SimConnectNative.AddClientEventToNotificationGroup (hSimConnect, groupId, eventId, bMaskable));
			}
		}

		[Serializable]
		internal class SetNotificationGroupPriority : MethodCall
		{
			private readonly uint groupId;
			private readonly uint priority;

			public SetNotificationGroupPriority (uint groupId, uint priority)
			{
				this.groupId = groupId;
				this.priority = priority;
			}

			protected override ReturnValue InternalInvoke (IntPtr hSimConnect)
			{
				return new ReturnValue (SimConnectNative.SetNotificationGroupPriority (hSimConnect, groupId, priority));
			}
		}

		[Serializable]
		internal class AddToDataDefinition : MethodCall
		{
			private readonly uint defineId;
			private readonly string datumName;
			private readonly string unitsName;
			private readonly Datatype datumType;
			private readonly float fEpsilon;
			private readonly uint datumId;

			public AddToDataDefinition (uint defineId, string datumName, string unitsName, Datatype datumType, float fEpsilon, uint datumId)
			{
				this.defineId = defineId;
				this.datumName = datumName;
				this.unitsName = unitsName;
				this.datumType = datumType;
				this.fEpsilon = fEpsilon;
				this.datumId = datumId;
			}

			protected override ReturnValue InternalInvoke (IntPtr hSimConnect)
			{
				return new ReturnValue (SimConnectNative.AddToDataDefinition (hSimConnect, defineId, datumName, unitsName, datumType, fEpsilon, datumId));
			}
		}

		[Serializable]
		internal class RequestDataOnSimObject : MethodCall
		{
			private readonly uint requestId;
			private readonly uint defineId;
			private readonly uint objectId;
			private readonly Period period;
			private readonly DataRequestFlag flags;
			private readonly uint origin;
			private readonly uint interval;
			private readonly uint limit;

			public RequestDataOnSimObject (uint requestId, uint defineId, uint objectId, Period period, DataRequestFlag flags, uint origin, uint interval, uint limit)
			{
				this.requestId = requestId;
				this.defineId = defineId;
				this.objectId = objectId;
				this.period = period;
				this.flags = flags;
				this.origin = origin;
				this.interval = interval;
				this.limit = limit;
			}

			protected override ReturnValue InternalInvoke (IntPtr hSimConnect)
			{
				return new ReturnValue (SimConnectNative.RequestDataOnSimObject (hSimConnect, requestId, defineId, objectId, period, flags, origin, interval, limit));
			}
		}

		[Serializable]
		internal class TransmitClientEvent : MethodCall
		{
			private readonly uint objectId;
			private readonly uint eventId;
			private readonly uint dwData;
			private readonly uint groupId;
			private readonly EventFlag flags;

			public TransmitClientEvent (uint objectId, uint eventId, uint dwData, uint groupId, EventFlag flags)
			{
				this.objectId = objectId;
				this.eventId = eventId;
				this.dwData = dwData;
				this.groupId = groupId;
				this.flags = flags;
			}

			protected override ReturnValue InternalInvoke (IntPtr hSimConnect)
			{
				return new ReturnValue (SimConnectNative.TransmitClientEvent (hSimConnect, objectId, eventId, dwData, groupId, flags));
			}
		}

		[Serializable]
		internal class GetNextDispatch : MethodCall
		{
			public GetNextDispatch ()
			{

			}

			protected override ReturnValue InternalInvoke (IntPtr hSimConnect)
			{
				IntPtr pData = IntPtr.Zero;
				byte [] data = null;
				uint cbData = 0;

				var hr = SimConnectNative.GetNextDispatch (hSimConnect, ref pData, ref cbData);

				if (hr >= 0)
				{
					data = new byte [cbData];
					Marshal.Copy (pData, data, 0, (int)cbData);
				}

				return new ReturnValue.GetNextDispatch (hr, data, cbData);
			}
		}

		[Serializable]
		internal class ReturnValue
		{
			private readonly int hresult;

			public ReturnValue (int hresult)
			{
				this.hresult = hresult;
			}

			public ReturnValue (ReturnValue other)
			{
				this.hresult = other.hresult;
			}

			public int Hresult => hresult;

			internal byte [] ToByteArray ()
			{
				return Serialize.ToByteArray (this);
			}

			static internal ReturnValue FromByteArray (byte [] arrBytes)
			{
				return Serialize.FromByteArray<ReturnValue> (arrBytes);
			}

			[Serializable]
			internal class GetNextDispatch : ReturnValue
			{
				private readonly byte [] data;
				private readonly uint cbData = 0;

				public GetNextDispatch (int hresult, byte [] data, uint cbData) : base (hresult)
				{
					this.data = data;
					this.cbData = cbData;
				}

				public byte [] Data => data;
				public uint CbData => cbData;
			}

			internal class Open : ReturnValue
			{
				private readonly IntPtr hSimConnect;

				public Open (int hresult, IntPtr hSimConnect) : base (hresult)
				{
					this.hSimConnect = hSimConnect;
				}

				public IntPtr HSimConnect => hSimConnect;
			}
		}
	}
}
