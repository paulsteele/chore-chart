using System;
using System.Collections.Generic;

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
			
			protected void SetAndNotify<T>(ref T backingField, T value)
			{
				if (EqualityComparer<T>.Default.Equals(backingField, value)) return;
				backingField = value;
				NotifyStateChanged();
			}
	}
}
