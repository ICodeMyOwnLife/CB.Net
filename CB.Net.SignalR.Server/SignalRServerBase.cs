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
        protected SignalRServerBase(SignalRConfiguration configuration): this(configuration.ConfigurationSection.Url) { }

        protected SignalRServerBase(string signalRUrl)
        {
            Url = signalRUrl;
        }
        #endregion


        #region  Properties & Indexers
        public virtual bool CanStart => State == SignalRState.Disconnected;
        public virtual bool CanStop => State == SignalRState.Connected;
        public virtual Exception Error { get; private set; }
        public virtual SignalRState State { get; private set; } = SignalRState.Disconnected;
        public string Url { get; }
        #endregion


        #region Methods
        public virtual async Task<bool> Start()
        {
            if (!CanStart)
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
                Error = exception;
                return false;
            }
        }

        public virtual bool Stop()
        {
            if (!CanStop)
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
            => Error = new Exception($"Service is {State.ToString().ToLower()}");
        #endregion
    }
}