namespace ThrottleApp
{
	class MainPageViewModel : BaseViewModel
	{
		private int trimValue = 20;
		public int TrimValue
		{
			get { return this.trimValue; }
			set { this.SetPropertyValue (ref this.trimValue, value); }
		}

		private int trimValueRequest = 20;
		public int TrimValueRequest
		{
			get { return this.trimValueRequest; }
			set { this.SetPropertyValue (ref this.trimValueRequest, value); }
		}

		private int throttleValue = 0;
		public int ThrottleValue
		{
			get { return this.throttleValue; }
			set { this.SetPropertyValue (ref this.throttleValue, value); }
		}

		private int throttleValueRequest = 0;
		public int ThrottleValueRequest
		{
			get { return this.throttleValueRequest; }
			set { this.SetPropertyValue (ref this.throttleValueRequest, value); }
		}

		private int landingGearValue = 0;
		public int LandingGearValue
		{
			get { return this.landingGearValue; }
			set { this.SetPropertyValue (ref this.landingGearValue, value); }
		}

		private int landingGearValueRequest = 0;
		public int LandingGearValueRequest
		{
			get { return this.landingGearValueRequest; }
			set { this.SetPropertyValue (ref this.landingGearValueRequest, value); }
		}

		private int flapsNumHandlePositions = 2;
		public int FlapsNumHandlePositions
		{
			get { return this.flapsNumHandlePositions; }
			set { this.SetPropertyValue (ref this.flapsNumHandlePositions, value); }
		}

		private int flapsHandleIndex = 20;
		public int FlapsHandleIndex
		{
			get { return this.flapsHandleIndex; }
			set { this.SetPropertyValue (ref this.flapsHandleIndex, value); }
		}

		private int flapsHandleIndexRequest = 20;
		public int FlapsHandleIndexRequest
		{
			get { return this.flapsHandleIndexRequest; }
			set { this.SetPropertyValue (ref this.flapsHandleIndexRequest, value); }
		}

		private int parkingBrakeValue = 0;
		public int ParkingBrakeValue
		{
			get { return this.parkingBrakeValue; }
			set { this.SetPropertyValue (ref this.parkingBrakeValue, value); }
		}

		private int parkingBrakeValueRequest = 0;
		public int ParkingBrakeValueRequest
		{
			get { return this.parkingBrakeValueRequest; }
			set { this.SetPropertyValue (ref this.parkingBrakeValueRequest, value); }
		}
	}
}
