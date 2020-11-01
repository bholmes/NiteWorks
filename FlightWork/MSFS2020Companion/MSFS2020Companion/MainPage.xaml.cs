using Xamarin.Forms;

namespace MSFS2020Companion
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
