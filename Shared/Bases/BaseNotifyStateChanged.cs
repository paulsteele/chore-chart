using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace hub.Shared.Bases {
	public interface INotifyStateChanged
	{
		event Action StateChanged;
	}
	public class BaseNotifyStateChanged : INotifyStateChanged{
			public event Action StateChanged;

			protected void NotifyStateChanged()
			{
				StateChanged?.Invoke();
			}
			
			protected void SetAndNotify<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
			{
				if (EqualityComparer<T>.Default.Equals(backingField, value)) return;
				backingField = value;
				NotifyStateChanged();
			}
	}
}
