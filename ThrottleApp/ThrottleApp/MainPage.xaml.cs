using Xamarin.Forms;

namespace ThrottleApp
{
	public partial class MainPage : ContentPage
	{
		ClientControllerBase clientController;

		public MainPage ()
		{
			InitializeComponent ();
			clientController = new ClientController ();
			BindingContext = clientController.ViewModel;
		}
	}
}
