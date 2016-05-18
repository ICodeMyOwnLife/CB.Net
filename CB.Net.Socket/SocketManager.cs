using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace CB.Net.Socket
{
    public class SocketManager
    {
        #region  Properties & Indexers
        public AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;
        public int Backlog { get; set; } = 100;
        public string IpAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8080;
        public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;
        public SocketType SocketType { get; set; } = SocketType.Stream;
        #endregion


        #region Methods
        public IPEndPoint CreateEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(IpAddress), Port);
        }

        public System.Net.Sockets.Socket CreateListener()
        {
            var server = CreateSocket();
            var localEndPoint = CreateEndPoint();
            server.Bind(localEndPoint);
            server.Listen(Backlog);
            return server;
        }

        public System.Net.Sockets.Socket CreateSender()
        {
            var sender = CreateSocket();
            var remoteEndPoint = CreateEndPoint();
            sender.Connect(remoteEndPoint);
            return sender;
        }

        public async Task<System.Net.Sockets.Socket> CreateSenderAsync()
        {
            var sender = CreateSocket();
            var remoteEndPoint = CreateEndPoint();
            await sender.ConnectAsync(remoteEndPoint);
            return sender;
        }

        public System.Net.Sockets.Socket CreateSocket()
            => new System.Net.Sockets.Socket(AddressFamily, SocketType, ProtocolType);
        #endregion
    }
}