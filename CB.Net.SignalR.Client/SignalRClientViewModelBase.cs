using System.Threading.Tasks;
using System.Windows.Input;
using CB.Model.Prism;
using Microsoft.Practices.Prism.Commands;


namespace CB.Net.SignalR.Client
{
    public class SignalRClientViewModelBase<TSignalRProxy>: PrismViewModelBase where TSignalRProxy: SignalRProxyBase
    {
        #region Fields
        protected readonly TSignalRProxy _proxy;
        #endregion


        #region  Constructors & Destructor
        public SignalRClientViewModelBase(TSignalRProxy proxy)
        {
            _proxy = proxy;
            _proxy.Error += Proxy_Error;
            ConnectAsyncCommand = DelegateCommand.FromAsyncHandler(ConnectAsync);
            DisconnectCommand = new DelegateCommand(Disconnect);
        }
        #endregion


        #region  Properties & Indexers
        public ICommand ConnectAsyncCommand { get; }
        public ICommand DisconnectCommand { get; }
        public NotificationRequestProvider NotificationRequestProvider { get; } = new NotificationRequestProvider();
        #endregion


        #region Methods
        public async Task ConnectAsync() => await _proxy.ConnectAsync();
        public void Disconnect() => _proxy.Disconnect();
        #endregion


        #region Event Handlers
        private void Proxy_Error(object sender, string e)
        {
            NotificationRequestProvider.NotifyError(e);
        }
        #endregion
    }
}