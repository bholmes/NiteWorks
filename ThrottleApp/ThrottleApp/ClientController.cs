using SimConnect;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ThrottleApp
{
	class ClientController
	{
		SocketClient simConnect;
		const float magicSimConnectNumber = 16383f;

		object trimLock = new object();
		Action nextTrimCall = null;

		public ClientController ()
		{
			simConnect = new SocketClient ("Managed Data Request", "GamerPC2020", 11000);

			simConnect.MapClientEventToSimEvent (MyEvents.ElevatorTrimSet, "ELEVATOR_TRIM_SET");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.ElevatorTrimSet);

			simConnect.MapClientEventToSimEvent (MyEvents.ThrottleSet, "THROTTLE_SET");
			simConnect.AddClientEventToNotificationGroup (MyGroups.Default, MyEvents.ThrottleSet);

			simConnect.AddToDataDefinition (MyDefinitions.TrimValue, "ELEVATOR TRIM INDICATOR", "Position", Datatype.Float32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.TrimValue, typeof (TrimValue));

			simConnect.AddToDataDefinition (MyDefinitions.ThrottleValue, "GENERAL ENG THROTTLE LEVER POSITION:1", "Percent over 100", Datatype.Float32, 0f, Constants.Unused);
			simConnect.RegisterStructToDataDefinition (MyDefinitions.ThrottleValue, typeof (ThrottleValue));

			simConnect.RequestDataOnSimObject (MyRequests.TrimValueFetch, MyDefinitions.TrimValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
			simConnect.RequestDataOnSimObject (MyRequests.ThrottleValueFetch, MyDefinitions.ThrottleValue, Constants.ObjectIdUser, Period.VisualFrame, DataRequestFlag.Changed);
			StartLoop ();
		}

		void StartLoop ()
		{
			Task.Run (() =>
			{
				while (true)
				{
					Action action;
					lock (trimLock)
					{
						action = nextTrimCall;
						nextTrimCall = null;
					}
					action?.Invoke ();

					try
					{
						var nextDispatch = simConnect.GetNextDispatch ();
						switch (nextDispatch.Id)
						{
							case RecvId.SimobjectData:
								var data = nextDispatch as RecvSimObjectData;
								switch ((MyRequests)data.RequestId)
								{
									case MyRequests.TrimValueFetch:
										var trimValue = (TrimValue)data.Data;
										SetTrimValue ((trimValue.TrimPosition + 1f) / 2f);
										break;

									case MyRequests.ThrottleValueFetch:
										var throttleValue = (ThrottleValue)data.Data;
										SetThrottleValue (throttleValue.GeneralEngThrottleLeverPosition1);
										break;
								}
								break;
							default:
								break;
						}
					}
					catch (Exception)
					{
						Thread.Sleep (10);
					}
				}
			});
		}

		private TestModel viewModel;
		public TestModel ViewModel
		{
			get => viewModel;

			set
			{
				if (viewModel == value)
					return;

				if (viewModel != null)
					viewModel.PropertyChanged -= ViewModel_PropertyChanged;

				viewModel = value;
				viewModel.PropertyChanged += ViewModel_PropertyChanged;
			}
		}

		void ViewModel_PropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof (TestModel.TrimValueRequest))
				SendTrimRequest (viewModel.TrimValueRequest);
			else if (e.PropertyName == nameof (TestModel.ThrottleValueRequest))
				SendThrottleRequest (viewModel.ThrottleValueRequest);
		}

		void SendTrimRequest (float trimValueRequest)
		{
			Action nextCall = () =>
			{
				var convertedValue = (int)Math.Round ((2 * magicSimConnectNumber * trimValueRequest) - magicSimConnectNumber);
				unchecked
				{
					simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ElevatorTrimSet, (uint)convertedValue, MyGroups.Default, EventFlag.Default);
				}
			};

			lock (trimLock)
				nextTrimCall = nextCall;
		}

		void SendThrottleRequest (float throttleValueRequest)
		{
			Action nextCall = () =>
			{
				var convertedValue = (int)Math.Round (magicSimConnectNumber * throttleValueRequest);
				unchecked
				{
					simConnect.TransmitClientEvent (Constants.ObjectIdUser, MyEvents.ThrottleSet, (uint)convertedValue, MyGroups.Default, EventFlag.Default);
				}
			};

			lock (trimLock)
				nextTrimCall = nextCall;
		}

		void SetTrimValue (float newValue)
		{
			var cache = viewModel;
			if (cache != null)
				Device.InvokeOnMainThreadAsync (() => cache.TrimValue = newValue);
		}

		void SetThrottleValue (float newValue)
		{
			var cache = viewModel;
			if (cache != null)
				Device.InvokeOnMainThreadAsync (() => cache.ThrottleValue = newValue);
		}
	}
}
