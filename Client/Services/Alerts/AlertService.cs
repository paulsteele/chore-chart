using hub.Shared.Bases;

namespace hub.Client.Services.Alerts
{
    public interface IAlertService : INotifyStateChanged
    {
        string CurrentTitle { get; }
        string CurrentMessage { get; }
        void ShowAlert(string title, string message);
        void DismissAlert();
    }

    public class AlertService : BaseNotifyStateChanged, IAlertService
    {
        public string CurrentTitle { get; private set; }
        public string CurrentMessage { get; private set; }

        public void ShowAlert(string title, string message)
        {
            CurrentTitle = title;
            CurrentMessage = message;
            NotifyStateChanged();
        }

        public void DismissAlert()
        {
            CurrentTitle = null;
            CurrentMessage = null;
            NotifyStateChanged();
        }
    }
}