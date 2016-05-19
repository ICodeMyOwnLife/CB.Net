using System;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;


namespace CB.Net.SignalR.Server
{
    public abstract class SignalRServerBase
    {
        #region Fields
        protected IDisposable _service;
        #endregion


        #region  Constructors & Destructor
        protected SignalRServerBase(SignalRConfiguration configuration) : this(configuration.ConfigurationSection.Url) { }
        protected SignalRServerBase(string signalRUrl)
        {
            Url = signalRUrl;
        }
        #endregion


        #region  Properties & Indexers
        public virtual bool CanStart => State == SignalRState.Disconnected;
        public virtual bool CanStop => State == SignalRState.Connected;
        public virtual string Error { get; private set; }
        public virtual SignalRState State { get; private set; } = SignalRState.Disconnected;
        public string Url { get; }
        #endregion


        #region Methods
        public virtual async Task<bool> Start()
        {
            if (State != SignalRState.Disconnected)
            {
                SetStateError();
                return false;
            }

            try
            {
                State = SignalRState.Connecting;
                _service = await Task.Run(() => WebApp.Start(Url));
                State = SignalRState.Connected;
                Error = null;
                return true;
            }
            catch (Exception exception)
            {
                State = SignalRState.Disconnected;
                Error = exception.Message;
                return false;
            }
        }

        public virtual bool Stop()
        {
            if (State != SignalRState.Connected)
            {
                SetStateError();
                return false;
            }

            _service.Dispose();
            State = SignalRState.Disconnected;
            Error = null;
            return true;
        }
        #endregion


        #region Implementation
        protected virtual void SetStateError()
        {
            Error = $"Service is {State.ToString().ToLower()}";
        }
        #endregion
    }
}