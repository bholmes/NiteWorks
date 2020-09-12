using System.Threading.Tasks;
using SimConnect;

namespace HelloWorld
{
	class ServerProgram
	{
		static void Main (string [] args)
		{
			var tList = new Task [2];
			tList [0] = Task.Run (() => ServerProgram.ServerMain (args));
			Task.Delay (1000);
			tList [1] = Task.Run (() => ClientProgram.ClientMain (args));

			Task.WaitAll (tList);
		}

		static void ServerMain (string [] args)
		{
			var server = new Server ();
			server.StartServer (11000).Wait ();
		}
	}
}
