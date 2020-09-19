using System.Threading;

namespace ThrottleApp
{
	class DevModeClientController : ClientControllerBase
	{
		protected override void SendThrottleRequest (float throttleValueRequest)
		{
			Thread.Sleep (10);
			SetThrottleValue (throttleValueRequest);
		}

		protected override void SendTrimRequest (float trimValueRequest)
		{
			Thread.Sleep (10);
			SetTrimValue (trimValueRequest);
		}
	}
}
