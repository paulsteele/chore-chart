using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace hub.Shared.Bases {
	public class BaseNotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;

			protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
			protected void SetValue<T>(ref T backingFiled, T value, [CallerMemberName] string propertyName = null)
			{
				if (EqualityComparer<T>.Default.Equals(backingFiled, value)) return;
				backingFiled = value;
				OnPropertyChanged(propertyName);
			}
	}
}
