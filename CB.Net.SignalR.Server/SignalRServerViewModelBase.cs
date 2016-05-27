using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CB.Model.Common;
using CB.Model.Prism;
using Microsoft.Practices.Prism.Commands;


namespace CB.Net.SignalR.Server
{
    public abstract class SignalRServerViewModelBase<TSignalRServer>: PrismViewModelBase, ILog
        where TSignalRServer: SignalRServerBase
    {
        #region Fields
        protected readonly TSignalRServer _server;
        #endregion


        #region  Constructors & Destructor
        protected SignalRServerViewModelBase(TSignalRServer server)
        {
            _server = server;
            StartServerAsyncCommand = DelegateCommand.FromAsyncHandler(StartServerAsync, () => CanStartServer);
            StopServerCommand = new DelegateCommand(StopServer, () => CanStopServer);
        }
        #endregion


        #region Abstract
        public abstract void Log(string logContent);
        public abstract void LogError(Exception exception);
        #endregion


        #region  Properties & Indexers
        public bool CanStartServer => _server.CanStart;
        public bool CanStopServer => _server.CanStop;
        public ICommand StartServerAsyncCommand { get; }
        public ICommand StopServerCommand { get; }
        #endregion


        #region Methods
        public async Task StartServerAsync()
        {
            Log("Connecting...");
            if (await _server.Start()) Log($"Connected to {_server.Url}");
            else LogError(_server.Error);
        }

        public void StopServer()
        {
            if (_server.Stop()) Log("Disconnected");
            else LogError(_server.Error);
        }
        #endregion
    }
}