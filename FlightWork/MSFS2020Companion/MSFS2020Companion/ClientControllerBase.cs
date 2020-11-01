using System.ComponentModel;
using Xamarin.Forms;
using MSFS2020Companion.Controls;

namespace MSFS2020Companion
{
	abstract class ClientControllerBase
	{
		protected ControlBase [] controls = new ControlBase[] {
			new TrimControl (), new ThrottleControl (), new LandingGearControl (),
			new FlapsControl (), new ParkingBrakeControl () };

		protected ClientControllerBase ()
		{
			viewModel = new MainPageViewModel ();

			foreach (var ctrl in controls)
			{
				ctrl.AssignViewModelProperties (viewModel);
				ctrl.RequestPropertyChanged += Control_RequestPropertyChanged;
			}
		}

		private void Control_RequestPropertyChanged (object sender, System.EventArgs e)
		{
			var ctrl = (ControlBase)sender;
			SendValueRequest (ctrl);
		}

		private MainPageViewModel viewModel;
		public MainPageViewModel ViewModel
		{
			get => viewModel;
		}

		protected abstract void SendValueRequest (ControlBase control);
	}
}
