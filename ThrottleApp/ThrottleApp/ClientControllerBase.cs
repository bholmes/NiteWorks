using System.ComponentModel;
using Xamarin.Forms;

namespace ThrottleApp
{
	abstract class ClientControllerBase
	{
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

		abstract protected void SendThrottleRequest (float throttleValueRequest);
		abstract protected void SendTrimRequest (float trimValueRequest);

		virtual protected void SetTrimValue (float newValue)
		{
			var cache = ViewModel;
			if (cache != null)
				Device.InvokeOnMainThreadAsync (() => cache.TrimValue = newValue);
		}

		virtual protected void SetThrottleValue (float newValue)
		{
			var cache = ViewModel;
			if (cache != null)
				Device.InvokeOnMainThreadAsync (() => cache.ThrottleValue = newValue);
		}
	}
}
