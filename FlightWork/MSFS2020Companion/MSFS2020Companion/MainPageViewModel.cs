namespace MSFS2020Companion
{
	class MainPageViewModel : BaseViewModel
	{
		public ModelItem Trim { get; set; }
		public ModelItem Throttle { get; set; }
		public ModelItem LandingGear { get; set; }
		public ModelItem FlapsNumHandlePositions { get; set; }
		public ModelItem FlapsHandleIndex { get; set; }
		public ModelItem ParkingBrake { get; set; }

		public class ModelItem : BaseViewModel
		{
			public ModelItem (int value)
			{
				this.value = value;
				this.valueRequest = value;
			}

			private int value;
			public int Value
			{
				get { return this.value; }
				set { this.SetPropertyValue (ref this.value, value); }
			}

			private int valueRequest;
			public int ValueRequest
			{
				get { return this.valueRequest; }
				set { this.SetPropertyValue (ref this.valueRequest, value); }
			}
		}
	}
}
