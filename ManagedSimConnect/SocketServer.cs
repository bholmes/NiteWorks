using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimConnect
{
	public class SocketServer
	{
		Socket socket;

		public Task StartServer (int port)
		{
			return Task.Run (() =>
			{
				// Data buffer for incoming data.  
				byte [] bytes = new Byte [1024];

				// Establish the local endpoint for the socket.  
				// Dns.GetHostName returns the name of the
				// host running the application.  

				Console.WriteLine (Dns.GetHostName ());

				IPHostEntry ipHostInfo = Dns.GetHostEntry (Dns.GetHostName ());
				IPAddress ipAddress = ipHostInfo.AddressList.Where (a => a.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault ();
				IPEndPoint localEndPoint = new IPEndPoint (ipAddress, port);

				// Create a TCP/IP socket.  
				Socket listener = new Socket (ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

				// Bind the socket to the local endpoint and
				// listen for incoming connections.  
				listener.Bind (localEndPoint);
				listener.Listen (10);

				socket = listener.Accept ();

				var buffSize = 1024;
				var buff = new byte [buffSize];

				var size = 0;

				while (size == 0)
				{
					size = socket.Receive (buff);
					Thread.Sleep (10);
				}

				// This should be a call to Open
				var openRet = (MethodCall.ReturnValue.Open)MethodCall.FromByteArray (buff).Invoke ();
				var hSimConnect = openRet.HSimConnect;
				socket.Send (new MethodCall.ReturnValue (openRet).ToByteArray ());

				if (hSimConnect == IntPtr.Zero)
					return; // In the future we go back to listening

				while (true)
				{
					size = socket.Receive (buff);
					if (size > 0)
						socket.Send (MethodCall.FromByteArray (buff).Invoke (hSimConnect).ToByteArray ());
					else
						Thread.Sleep (10);
				}

				//socket.Shutdown (SocketShutdown.Both);
				//socket.Close ();

			});
		}


	}
}
