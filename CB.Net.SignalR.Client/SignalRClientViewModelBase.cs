using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CB.Model.Common;
using CB.Model.Prism;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Practices.Prism.Commands;


namespace CB.Net.SignalR.Client
{
    public abstract class SignalRClientViewModelBase<TSignalRProxy>: PrismViewModelBase, ILog
        where TSignalRProxy: SignalRProxyBase, new()
    {
        #region Fields
        private bool _canConnect = true;
        private bool _canDisconnect;
        protected TSignalRProxy _proxy;
        #endregion


        #region  Constructors & Destructor
        protected SignalRClientViewModelBase()
        {
            ConnectAsyncCommand = DelegateCommand.FromAsyncHandler(ConnectAsync, () => CanConnect);
            DisconnectCommand = new DelegateCommand(Disconnect, () => CanDisconnect);
        }
        #endregion


        #region  Properties & Indexers
        public virtual bool CanConnect
        {
            get { return _canConnect; }
            protected set
            {
                if (SetProperty(ref _canConnect, value))
                {
                    RaiseConnectCanExecuteChanged();
                }
            }
        }

        public virtual bool CanDisconnect
        {
            get { return _canDisconnect; }
            protected set
            {
                if (SetProperty(ref _canDisconnect, value))
                {
                    RaiseConnectCanExecuteChanged();
                }
            }
        }

        public ICommand ConnectAsyncCommand { get; }
        public ICommand DisconnectCommand { get; }
        public NotificationRequestProvider NotificationRequestProvider { get; } = new NotificationRequestProvider();
        #endregion


        #region Methods
        public async Task ConnectAsync()
        {
            if (!CanConnect) return;

            CanConnect = CanDisconnect = false;
            _proxy = new TSignalRProxy();
            _proxy.InitializeProxy(InitializeProxy);
            try
            {
                await _proxy.ConnectAsync();
                CanDisconnect = true;
            }
            catch (Exception exception)
            {
                LogError(exception);
                CanConnect = true;
            }
        }

        public void Disconnect()
        {
            if (!CanDisconnect) return;

            CanConnect = CanDisconnect = false;
            try
            {
                _proxy.Disconnect();
                _proxy.Dispose();
                CanConnect = true;
            }
            catch (Exception exception)
            {
                LogError(exception);
                CanDisconnect = true;
            }
            
        }
        #endregion


        #region Implementation
        protected virtual void InitializeProxy(IHubProxy proxy) { }

        private void Proxy_Error(object sender, Exception exception)
        {
            NotificationRequestProvider.NotifyError(exception.Message);
        }

        protected virtual void RaiseConnectCanExecuteChanged()
        {
            RaiseCommandsCanExecuteChanged(ConnectAsyncCommand, DisconnectCommand);
        }
        #endregion


        public abstract void Log(string logContent);
        public abstract void LogError(Exception exception);
    }
}