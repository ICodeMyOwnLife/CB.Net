using System.Threading.Tasks;
using CB.Model.Common;
using Microsoft.AspNet.SignalR;


namespace CB.Net.SignalR.Server
{
    public abstract class SignalRHubBase: Hub
    {
        #region Fields
        protected ILog _logger;
        #endregion


        #region  Constructors & Destructor
        protected SignalRHubBase(): this(null) { }

        protected SignalRHubBase(ILog logger)
        {
            _logger = logger;
        }
        #endregion


        #region Override
        public override Task OnConnected()
        {
            _logger?.Log($"{Context.ConnectionId} connected.");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _logger?.Log($"{Context.ConnectionId} disconnected{(stopCalled ? " (stop called)" : "")}.");
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            _logger.Log($"{Context.ConnectionId} reconnected");
            return base.OnReconnected();
        }
        #endregion
    }
}