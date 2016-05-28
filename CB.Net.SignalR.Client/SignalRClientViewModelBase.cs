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

        private string _logMessage;
        protected TSignalRProxy _proxy;
        #endregion


        #region  Constructors & Destructor
        protected SignalRClientViewModelBase()
        {
            ConnectAsyncCommand = DelegateCommand.FromAsyncHandler(ConnectAsync, () => CanConnect);
            DisconnectAsyncCommand = DelegateCommand.FromAsyncHandler(DisconnectAsync, () => CanDisconnect);
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
        public ICommand DisconnectAsyncCommand { get; }

        public virtual string LogMessage
        {
            get { return _logMessage; }
            protected set { SetProperty(ref _logMessage, value); }
        }

        public NotificationRequestProvider NotificationRequestProvider { get; } = new NotificationRequestProvider();
        #endregion


        #region Methods
        public async Task ConnectAsync()
        {
            if (!CanConnect) return;

            InitializeProxy();
            await _proxy.ConnectAsync();
        }

        public async Task DisconnectAsync()
        {
            if (!CanDisconnect) return;

            await _proxy.DisconnectAsync();
            TerminateProxy();
        }

        public virtual void Log(string logContent)
            => LogMessage = LogMessage == null ? logContent : LogMessage + Environment.NewLine + logContent;

        public virtual void LogError(Exception exception)
        {
            NotificationRequestProvider.NotifyError(exception.Message);
        }
        #endregion


        #region Event Handlers
        protected virtual void OnProxyError(Exception exception)
        {
            NotificationRequestProvider.NotifyError(exception.Message);
        }

        protected virtual void OnProxyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SignalRProxyBase.ConnectionState):
                    SetConnectAbility();
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
            _proxy.Error += OnProxyError;
            _proxy.PropertyChanged += OnProxyPropertyChanged;
        }

        protected virtual void RaiseConnectCanExecuteChanged()
        {
            RaiseCommandsCanExecuteChanged(ConnectAsyncCommand, DisconnectAsyncCommand);
        }

        private void SetConnectAbility()
        {
            CanConnect = _proxy.CanConnect();
            CanDisconnect = _proxy.CanDisconnect();
        }

        private void TerminateProxy()
        {
            _proxy.Error -= OnProxyError;
            _proxy.PropertyChanged -= OnProxyPropertyChanged;
            _proxy.Dispose();
        }
        #endregion
    }
}