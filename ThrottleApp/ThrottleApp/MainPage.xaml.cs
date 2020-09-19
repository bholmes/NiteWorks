using Xamarin.Forms;

namespace ThrottleApp
{
	public partial class MainPage : ContentPage
	{
		ClientControllerBase clientController;

		public MainPage ()
		{
			InitializeComponent ();

			clientController = new ClientController { ViewModel = (TestModel)BindingContext };
		}
	}
}
