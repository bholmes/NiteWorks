using SimConnect;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace HelloWorld
{
	enum MyEvents
	{
		Brakes, //BRAKES
		ElevatorTrimSet, //ELEVATOR_TRIM_SET
		ElevTrimDown, //ELEV_TRIM_DN
		ElevTrimUp, //ELEV_TRIM_UP
	}

	enum MyGroups
	{
		One
	}

	enum MyDefinitions
	{
		One,
		Two,
	}

	enum MyRequests
	{
		One,
		Two,
	}

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	struct Struct1
	{
		public float TrimIndicator;
		public float TrimPosition;
	}

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	struct Struct2
	{
		public float VelocityBodyY;
		public float VelocityWorldY;
		public float VerticalSpeed;
		public float TrimPosition;
	}

	class ClientProgram
	{
		SocketClient client;
		bool running = true;

		private void Run ()
		{
			client = new SocketClient ("Managed Data Request", System.Net.Dns.GetHostName (), 11000);

			client.MapClientEventToSimEvent (MyEvents.Brakes, "BRAKES");
			client.AddClientEventToNotificationGroup (MyGroups.One, MyEvents.Brakes);

			client.MapClientEventToSimEvent (MyEvents.ElevatorTrimSet, "ELEVATOR_TRIM_SET");
			client.AddClientEventToNotificationGroup (MyGroups.One, MyEvents.ElevatorTrimSet);

			client.MapClientEventToSimEvent (MyEvents.ElevTrimDown, "ELEV_TRIM_DN");
			client.AddClientEventToNotificationGroup (MyGroups.One, MyEvents.ElevTrimDown);

			client.MapClientEventToSimEvent (MyEvents.ElevTrimUp, "ELEV_TRIM_UP");
			client.AddClientEventToNotificationGroup (MyGroups.One, MyEvents.ElevTrimUp);

			client.SetNotificationGroupPriority (MyGroups.One, GroupPriority.Highest);

			client.AddToDataDefinition (MyDefinitions.One, "ELEVATOR TRIM INDICATOR", "Position", Datatype.Float32, 0f, Constants.Unused);
			client.AddToDataDefinition (MyDefinitions.One, "ELEVATOR TRIM POSITION", "Degrees", Datatype.Float32, 0f, Constants.Unused);

			client.AddToDataDefinition (MyDefinitions.Two, "VELOCITY BODY Y", "Feet per second", Datatype.Float32, 0f, Constants.Unused);
			client.AddToDataDefinition (MyDefinitions.Two, "VELOCITY WORLD Y", "Feet per second", Datatype.Float32, 0f, Constants.Unused);
			client.AddToDataDefinition (MyDefinitions.Two, "VERTICAL SPEED", "Feet per second", Datatype.Float32, 0f, Constants.Unused);
			client.AddToDataDefinition (MyDefinitions.Two, "ELEVATOR TRIM POSITION", "Degrees", Datatype.Float32, 0f, Constants.Unused);

			client.RequestDataOnSimObject (MyRequests.Two, MyDefinitions.Two, Constants.ObjectIdUser, Period.Second, DataRequestFlag.Changed);

    //        unchecked
    //        {
				//client.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ElevatorTrimSet, (uint)0, MyGroups.One, EventFlag.Default);
    //        }

            var buff = new byte[1024];

			while (running)
			{
				uint sizeRead;
				var hr = client.GetNextDispatch (buff, out sizeRead);
				if (hr >= 0)
					OnReceiveFromServer (buff, sizeRead);
				else
					Thread.Sleep (10);
			}
		}

		unsafe private void OnReceiveFromServer (byte [] buff, uint size)
		{
			fixed (byte* b = &buff [0])
			{
				var pData = (IntPtr)b;

				var pRecv = Marshal.PtrToStructure<SIMCONNECT_RECV> (pData);
				switch ((RecvId)pRecv.dwID)
				{
					case RecvId.Event:
						{
							// enter code to handle events received in a SIMCONNECT_RECV_EVENT structure.
							var evt = Marshal.PtrToStructure<SIMCONNECT_RECV_EVENT> (pData);

							switch ((MyEvents)evt.uEventID)
							{
								case MyEvents.Brakes:
									Console.WriteLine ("MyEvents.Brakes {0}", evt.dwData);
									break;

								case MyEvents.ElevatorTrimSet:
									Console.WriteLine ("MyEvents.ElevatorTrimSet {0}", (int)evt.dwData);
									client.RequestDataOnSimObject (MyRequests.One, MyDefinitions.One, Constants.ObjectIdUser, Period.Once, DataRequestFlag.Default);
									break;

								case MyEvents.ElevTrimUp:
									Console.WriteLine ("MyEvents.ElevTrimUp {0}", (int)evt.dwData);
									client.RequestDataOnSimObject (MyRequests.One, MyDefinitions.One, Constants.ObjectIdUser, Period.Once, DataRequestFlag.Default);
									break;

								case MyEvents.ElevTrimDown:
									Console.WriteLine ("MyEvents.ElevTrimDown {0}", (int)evt.dwData);
									client.RequestDataOnSimObject (MyRequests.One, MyDefinitions.One, Constants.ObjectIdUser, Period.Once, DataRequestFlag.Default);
									break;

								default:
									break;
							}
							break;
						}

					case RecvId.SimobjectData:
						{
							var data = Marshal.PtrToStructure<SIMCONNECT_RECV_SIMOBJECT_DATA> (pData);

							switch ((MyRequests)data.dwRequestID)
							{
								case MyRequests.One:
									var myStruct = Marshal.PtrToStructure<Struct1> (pData + (SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE.dwDataStart));
									Console.WriteLine ("TrimIndicator  = {0};		TrimPosition  = {1}", myStruct.TrimIndicator, myStruct.TrimPosition);
									break;

								case MyRequests.Two:
									var myStruct2 = Marshal.PtrToStructure<Struct2> (pData + (SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE.dwDataStart));
									Console.WriteLine ($"Speed:{myStruct2.VerticalSpeed * 60}, Trim:{myStruct2.TrimPosition}");
									break;

								default:
									break;

							}

							break;
						}


					case RecvId.Quit:
						// enter code to handle exiting the application
						running = false;
						break;

					default:
						break;
				}
			}
		}

		public static void ClientMain (string [] args)
		{
			new ClientProgram ().Run ();
		}
	}
}
