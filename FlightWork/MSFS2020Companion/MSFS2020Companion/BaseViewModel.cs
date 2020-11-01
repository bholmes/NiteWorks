using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MSFS2020Companion
{
	abstract class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected bool SetPropertyValue<T> (ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (value == null ? field != null : !value.Equals (field))
			{
				field = value;

				PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
				return true;
			}
			return false;
		}
	}
}
