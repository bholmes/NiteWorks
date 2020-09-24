using System.ComponentModel;
using Xamarin.Forms;

namespace ThrottleApp
{
	abstract class ClientControllerBase
	{
		protected MainPageViewModel viewModel;
		public MainPageViewModel ViewModel
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
			if (e.PropertyName == nameof (MainPageViewModel.TrimValueRequest))
				SendTrimRequest (viewModel.TrimValueRequest);
			else if (e.PropertyName == nameof (MainPageViewModel.ThrottleValueRequest))
				SendThrottleRequest (viewModel.ThrottleValueRequest);
			else if (e.PropertyName == nameof (MainPageViewModel.LandingGearValueRequest))
				SendLandingGearRequest (viewModel.LandingGearValueRequest);
			else if (e.PropertyName == nameof (MainPageViewModel.FlapsHandleIndexRequest))
				SetFlapsHandleIndexRequest (viewModel.FlapsHandleIndexRequest);
			else if (e.PropertyName == nameof (MainPageViewModel.ParkingBrakeValueRequest))
				SetParkingBrakeRequest (viewModel.ParkingBrakeValueRequest);
		}

		abstract protected void SendThrottleRequest (int throttleValueRequest);
		abstract protected void SendTrimRequest (int trimValueRequest);
		abstract protected void SendLandingGearRequest (int landingGearValueRequest);
		abstract protected void SetFlapsHandleIndexRequest (int flapsHandleIndexRequest);
		abstract protected void SetParkingBrakeRequest (int parkingBrakeValueRequest);

		virtual protected void SetTrimValue (int newValue)
		{
			var cache = ViewModel;
			if (cache != null)
				Device.InvokeOnMainThreadAsync (() => cache.TrimValue = newValue);
		}

		virtual protected void SetThrottleValue (int newValue)
		{
			var cache = ViewModel;
			if (cache != null)
				Device.InvokeOnMainThreadAsync (() => cache.ThrottleValue = newValue);
		}

		virtual protected void SetLandingGearValue (int newValue)
		{
			var cache = ViewModel;
			if (cache != null)
				Device.InvokeOnMainThreadAsync (() => cache.LandingGearValue = newValue);
		}

		virtual protected void SetFlapsNumHandlePositions (int newValue)
		{
			var cache = ViewModel;
			if (cache != null)
				Device.InvokeOnMainThreadAsync (() => cache.FlapsNumHandlePositions = newValue);
		}

		virtual protected void SetFlapsHandleIndex (int newValue)
		{
			var cache = ViewModel;
			if (cache != null)
				Device.InvokeOnMainThreadAsync (() => cache.FlapsHandleIndex = newValue);
		}

		virtual protected void SetParkingBrakeValue (int newValue)
		{
			var cache = ViewModel;
			if (cache != null)
				Device.InvokeOnMainThreadAsync (() => cache.ParkingBrakeValue = newValue);
		}
	}
}
