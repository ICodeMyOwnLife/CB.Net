using System;
using System.Threading.Tasks;
using CB.Model.Common;
using Microsoft.AspNet.SignalR.Client;


namespace CB.Net.SignalR.Client
{
    public abstract class SignalRProxyBase: BindableObject, IDisposable
    {
        #region Fields
        private SignalRState _connectionState = SignalRState.Disconnected;
        protected HubConnection _hubConnection;
        protected IHubProxy _hubProxy;
        #endregion


        #region  Constructors & Destructor
        protected SignalRProxyBase(SignalRConfiguration configuration)
            : this(configuration.ConfigurationSection.Url, configuration.ConfigurationSection.HubName) { }

        protected SignalRProxyBase(string signalRUrl, string hubName)
        {
            SignalRUrl = signalRUrl;
            HubName = hubName;
            _hubConnection = new HubConnection(SignalRUrl);
            _hubProxy = _hubConnection.CreateHubProxy(HubName);
        }
        #endregion


        #region  Properties & Indexers
        public virtual SignalRState ConnectionState
        {
            get { return _connectionState; }
            protected set { SetProperty(ref _connectionState, value); }
        }

        public string HubName { get; }
        public string SignalRUrl { get; }
        #endregion


        #region Events
        public event Action<Exception> Error;
        #endregion


        #region Methods
        public bool CanConnect()
            => ConnectionState == SignalRState.Disconnected;

        public bool CanDisconnect()
            => ConnectionState == SignalRState.Connected;

        public virtual async Task ConnectAsync()
        {
            if (!CanConnect())
            {
                OnStateError();
                return;
            }

            await TryAsync(async () =>
            {
                ConnectionState = SignalRState.Connecting;
                InitializeProxy();
                await _hubConnection.Start();
                ConnectionState = SignalRState.Connected;
            }, () => { ConnectionState = SignalRState.Disconnected; });
        }

        public virtual async Task DisconnectAsync()
        {
            if (!CanDisconnect())
            {
                OnStateError();
                return;
            }

            await TryAsync(async () =>
             {
                 await Task.Delay(0);
                 _hubConnection.Stop();
                 ConnectionState = SignalRState.Disconnected;
             }, () => ConnectionState = SignalRState.Connected);
        }

        public void Dispose()
            => _hubConnection?.Dispose();

        public virtual void InitializeProxy(Action<IHubProxy> initializationAction)
        {
            initializationAction(_hubProxy);
        }
        #endregion


        #region Event Handlers
        protected virtual void OnError(Exception exception)
            => Error?.Invoke(exception);
        #endregion


        #region Implementation
        protected virtual void InitializeProxy()
        {
            _hubConnection.Error += OnError;
            _hubProxy.On<Exception>("error", OnError);
        }

        protected virtual void OnError(string error)
            => OnError(new Exception(error));

        private void OnStateError()
            => OnError($"Proxy is {ConnectionState.ToString().ToLower()}");

        protected virtual void Try(Action action, Action catchAction = null)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                OnError(exception);
                catchAction?.Invoke();
            }
        }

        protected virtual async Task TryAsync(Func<Task> action, Action catchAction = null)
        {
            try
            {
                await action();
            }
            catch (Exception exception)
            {
                OnError(exception);
                catchAction?.Invoke();
            }
        }
        #endregion
    }
}