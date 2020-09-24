using System.Threading;

namespace ThrottleApp
{
	class DevModeClientController : ClientControllerBase
	{
		protected override void SendThrottleRequest (int throttleValueRequest)
		{
			Thread.Sleep (10);
			SetThrottleValue (throttleValueRequest);
		}

		protected override void SendTrimRequest (int trimValueRequest)
		{
			Thread.Sleep (10);
			SetTrimValue (trimValueRequest);
		}

		protected override void SendLandingGearRequest (int landingGearValueRequest)
		{
			Thread.Sleep (10);
			SetLandingGearValue (landingGearValueRequest);
		}

		protected override void SetFlapsHandleIndexRequest (int flapsHandleIndexRequest)
		{
			Thread.Sleep (10);
			SetFlapsHandleIndex (flapsHandleIndexRequest);
		}

		protected override void SetParkingBrakeRequest (int parkingBrakeValueRequest)
		{
			Thread.Sleep (10);
			SetParkingBrakeValue (parkingBrakeValueRequest);
		}
	}
}
