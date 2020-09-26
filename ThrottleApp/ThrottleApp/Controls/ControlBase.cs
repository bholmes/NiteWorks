using System;
using SimConnect;

namespace ThrottleApp.Controls
{
	abstract class ControlBase
	{
		public event RequestPropertyChangedEventHandler RequestPropertyChanged;

		protected ControlBase (MainPageViewModel.ModelItem modelItem, MyRequests requestId)
		{
			ModelItem = modelItem;
			RequestId = requestId;
			modelItem.PropertyChanged += ModelItem_PropertyChanged;
		}

		private void ModelItem_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof (ModelItem.ValueRequest))
				RequestPropertyChanged?.Invoke (this, new EventArgs ());
		}

		public abstract void AssignViewModelProperties (MainPageViewModel viewModel);
		public abstract void MapEvent (SocketClient simConnect);
		public abstract void RegisterDataDefinition (SocketClient simConnect);
		public abstract void StartDataRequest (SocketClient simConnect);
		public abstract void UpdateFromObjectData (RecvSimObjectData data);
		public abstract void TransmitUpdateRequest (SocketClient simConnect);

		public MainPageViewModel.ModelItem ModelItem { get; private set; }
		public MyRequests RequestId { get; private set; }
	}

	public delegate void RequestPropertyChangedEventHandler (object sender, EventArgs e);
}
