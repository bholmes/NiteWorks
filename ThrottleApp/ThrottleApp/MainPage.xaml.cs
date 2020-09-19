using Xamarin.Forms;

namespace ThrottleApp
{
	public partial class MainPage : ContentPage
	{
		ClientController clientController;

		public MainPage ()
		{
			InitializeComponent ();

			clientController = new ClientController { ViewModel = (TestModel)BindingContext };
		}
	}
}
