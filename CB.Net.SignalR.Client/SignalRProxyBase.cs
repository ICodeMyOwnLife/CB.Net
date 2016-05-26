using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;


namespace CB.Net.SignalR.Client
{
    public abstract class SignalRProxyBase
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
        }
        #endregion


        #region  Properties & Indexers
        public string HubName { get; }
        public string SignalRUrl { get; }
        #endregion


        #region Events
        public event EventHandler<Exception> Error;
        #endregion


        #region Methods
        public virtual async Task ConnectAsync()
        {
            InitializeProxy();
            await TryAsync(async () => await _hubConnection.Start());
        }

        public virtual void Disconnect()
            => Try(() => _hubConnection.Stop());
        #endregion


        #region Event Handlers
        protected virtual void OnError(Exception exception)
        {
            Error?.Invoke(this, exception);
        }
        #endregion


        #region Implementation
        protected virtual void InitializeProxy()
        {
            _hubConnection = new HubConnection(SignalRUrl);
            _hubConnection.Error += OnError;
            _hubProxy = _hubConnection.CreateHubProxy(HubName);
            _hubProxy.On<Exception>("error", OnError);
        }

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