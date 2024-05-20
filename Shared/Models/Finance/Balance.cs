using System.Collections.Generic;

namespace hub.Shared.Models.Finance;

public class Balance(decimal total, decimal leftToSpend, Dictionary<int, decimal> categorySpend)
{
	public decimal Total { get; } = total;
	public decimal LeftToSpend { get; } = leftToSpend;
	public Dictionary<int, decimal> CategorySpend { get; } = categorySpend;
}