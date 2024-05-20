using System.Collections.Generic;

namespace hub.Shared.Models.Finance;

public class Balance(decimal total, Dictionary<int, decimal> categorySpend)
{
	public decimal Total { get; } = total;
	public Dictionary<int, decimal> CategorySpend { get; } = categorySpend;
}