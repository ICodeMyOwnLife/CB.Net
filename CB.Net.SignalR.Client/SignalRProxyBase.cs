using System;
using System.Threading.Tasks;
using CB.Model.Common;
using Microsoft.AspNet.SignalR.Client;


namespace CB.Net.SignalR.Client
{
    public abstract class SignalRProxyBase: BindableObject, IDisposable
    {
        #region Fields
        protected HubConnection _hubConnection;
        protected IHubProxy _hubProxy;
        private SignalRState _state = SignalRState.Disconnected;
        #endregion


        #region  Constructors & Destructor
        protected SignalRProxyBase(SignalRConfiguration configuration)
            : this(configuration.ConfigurationSection.Url, configuration.ConfigurationSection.HubName) { }

        protected SignalRProxyBase(string signalRUrl, string hubName)
        {
            SignalRUrl = signalRUrl;
            HubName = hubName;
        }
        #endregion


        #region  Properties & Indexers
        public string HubName { get; }
        public string SignalRUrl { get; }

        public virtual SignalRState State
        {
            get { return _state; }
            protected set { SetProperty(ref _state, value); }
        }
        #endregion


        #region Events
        public event Action<Exception> Error;
        #endregion


        #region Methods
        public virtual async Task ConnectAsync()
            => await TryAsync(async () =>
            {
                State = SignalRState.Connecting;
                InitializeProxy();
                await _hubConnection.Start();
                State = SignalRState.Connected;
            });

        public virtual void Disconnect()
            => Try(() =>
            {
                _hubConnection.Stop();
                State = SignalRState.Disconnected;
            });

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
            _hubConnection = new HubConnection(SignalRUrl);
            _hubConnection.Error += OnError;
            _hubProxy = _hubConnection.CreateHubProxy(HubName);
            _hubProxy.On<Exception>("error", OnError);
        }

        protected virtual void OnError(string error)
            => OnError(new Exception(error));

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