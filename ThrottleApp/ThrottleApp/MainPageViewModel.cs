namespace ThrottleApp
{
	class TestModel : BaseViewModel
	{
		private float trimValue = .5f;
		public float TrimValue
		{
			get { return this.trimValue; }
			set { this.SetPropertyValue (ref this.trimValue, value); }
		}

		private float trimValueRequest = .5f;
		public float TrimValueRequest
		{
			get { return this.trimValueRequest; }
			set { this.SetPropertyValue (ref this.trimValueRequest, value); }
		}

		private float throttleValue = .5f;
		public float ThrottleValue
		{
			get { return this.throttleValue; }
			set { this.SetPropertyValue (ref this.throttleValue, value); }
		}

		private float throttleValueRequest = .5f;
		public float ThrottleValueRequest
		{
			get { return this.throttleValueRequest; }
			set { this.SetPropertyValue (ref this.throttleValueRequest, value); }
		}
	}
}
