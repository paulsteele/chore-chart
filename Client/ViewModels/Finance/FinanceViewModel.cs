using hub.Shared.Bases;

namespace hub.Client.ViewModels.Finance;

public interface IFinanceViewModel : INotifyStateChanged
{
}

public class FinanceViewModel : BaseNotifyStateChanged, IFinanceViewModel
{ }