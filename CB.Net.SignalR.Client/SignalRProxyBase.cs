using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;


namespace CB.Net.SignalR.Client
{
    public abstract class SignalRProxyBase: IDisposable
    {
        #region Fields
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

            // ReSharper disable once VirtualMemberCallInContructor
            InitializeProxy();
        }
        #endregion


        #region  Properties & Indexers
        public bool CanConnect => State == SignalRState.Disconnected;
        public bool CanDisconnect => State == SignalRState.Connected;
        public string HubName { get; }
        public string SignalRUrl { get; }
        public virtual SignalRState State { get; protected set; } = SignalRState.Disconnected;
        #endregion


        #region Events
        public event EventHandler<Exception> Error;
        #endregion


        #region Methods
        public virtual async Task ConnectAsync()
        {
            if (!CanConnect)
            {
                RaiseStateError();
                return;
            }
            await TryAsync(async () =>
            {
                State = SignalRState.Connecting;
                await _hubConnection.Start();
                State = SignalRState.Connected;
            });
        }

        public virtual void Disconnect()
        {
            if (!CanDisconnect)
            {
                RaiseStateError();
                return;
            }
            Try(() =>
            {
                _hubConnection.Stop();
                State = SignalRState.Disconnected;
            });
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
            => Error?.Invoke(this, exception);
        #endregion


        #region Implementation
        protected virtual void InitializeProxy()
        {
            _hubConnection = new HubConnection(SignalRUrl);
            _hubConnection.Error += OnError;
            _hubProxy = _hubConnection.CreateHubProxy(HubName);
            _hubProxy.On<Exception>("error", OnError);
        }

        protected virtual void OnError(string error)
            => OnError(new Exception(error));

        private void RaiseStateError()
            => OnError($"Hub proxy is {State.ToString().ToLower()}");

        protected virtual void Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                OnError(exception);
            }
        }

        protected virtual async Task TryAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception exception)
            {
                OnError(exception);
            }
        }
        #endregion
    }
}


// TODO: CanConnect, CanDisconnect