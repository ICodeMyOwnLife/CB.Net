using System;
using System.ComponentModel;
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


        #region Abstract
        public abstract void Log(string logContent);
        public abstract void LogError(Exception exception);
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

            try
            {
                CanConnect = CanDisconnect = false;
                InitializeProxy();
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

            try
            {
                CanConnect = CanDisconnect = false;
                _proxy.Disconnect();
                TerminateProxy();
                CanConnect = true;
            }
            catch (Exception exception)
            {
                LogError(exception);
                CanDisconnect = true;
            }
        }
        #endregion


        #region Event Handlers
        private void Proxy_Error(Exception exception)
        {
            NotificationRequestProvider.NotifyError(exception.Message);
        }

        private void Proxy_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SignalRProxyBase.State):
                    SetEnabilities();
                    break;
            }
        }
        #endregion


        #region Implementation
        protected virtual void InitializeHubProxy(IHubProxy proxy) { }

        protected virtual void InitializeProxy()
        {
            _proxy = new TSignalRProxy();
            _proxy.InitializeProxy(InitializeHubProxy);
            _proxy.Error += Proxy_Error;
            _proxy.PropertyChanged += Proxy_PropertyChanged;
        }

        protected virtual void RaiseConnectCanExecuteChanged()
        {
            RaiseCommandsCanExecuteChanged(ConnectAsyncCommand, DisconnectCommand);
        }

        private void SetEnabilities()
        {
            CanConnect = _proxy.State == SignalRState.Disconnected;
            CanDisconnect = _proxy.State == SignalRState.Connected;
        }

        private void TerminateProxy()
        {
            _proxy.Error -= Proxy_Error;
            _proxy.PropertyChanged -= Proxy_PropertyChanged;
            _proxy.Dispose();
        }
        #endregion
    }
}