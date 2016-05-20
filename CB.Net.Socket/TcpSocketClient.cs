using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CB.Model.Serialization;


namespace CB.Net.Socket
{
    public class TcpSocketClient
    {
        #region Fields
        private readonly int _bufferSize;
        private readonly string _ipAddress;
        private readonly int _port;
        #endregion


        #region  Constructors & Destructor
        public TcpSocketClient(TcpSocketConfiguration configuration)
            : this(configuration.IpAddress, configuration.Port, configuration.BufferSize) { }

        public TcpSocketClient(string ipAddress = TcpSocketParams.IP_ADDRESS, int port = TcpSocketParams.PORT,
            int bufferSize = TcpSocketParams.BUFFER_SIZE)
        {
            _ipAddress = ipAddress;
            _port = port;
            _bufferSize = bufferSize;
        }
        #endregion


        #region Methods
        public void SendFile(string filePath)
        {
            SendObject(new NetFileInfo(filePath));
            using (var readerStream = GetReaderStream(filePath))
            {
                SendData(readerStream);
            }
        }

        public async Task SendFileAsync(string filePath)
            => await SendFileAsync(filePath, CancellationToken.None);

        public async Task SendFileAsync(string filePath, CancellationToken cancellationToken)
        {
            await SendObjectAsync(new NetFileInfo(filePath), cancellationToken);
            using (var readerStream = GetReaderStream(filePath))
            {
                await SendDataAsync(readerStream, cancellationToken);
            }
        }

        public void SendObject<T>(T obj) => SendText(Serialize(obj));

        public async Task SendObjectAsync<T>(T obj)
            => await SendObjectAsync(obj, CancellationToken.None);

        public async Task SendObjectAsync<T>(T obj, CancellationToken cancellationToken)
            => await SendTextAsync(Serialize(obj), cancellationToken);

        public void SendText(string text) => SendData(GetTextData(text));

        public async Task SendTextAsync(string text)
            => await SendTextAsync(text, CancellationToken.None);

        public async Task SendTextAsync(string text, CancellationToken cancellationToken)
            => await SendDataAsync(GetTextData(text), cancellationToken);
        #endregion


        #region Implementation
        private TcpClient CreateTcpClient()
        {
            var client = new TcpClient(_ipAddress, _port);
            client.Connect(_ipAddress, _port);
            return client;
        }

        private async Task<TcpClient> CreateTcpClientAsync()
        {
            var client = new TcpClient(_ipAddress, _port);
            await client.ConnectAsync(_ipAddress, _port);
            return client;
        }

        private static FileStream GetReaderStream(string filePath)
        {
            return File.OpenRead(filePath);
        }

        private static byte[] GetTextData(string text)
            => Encoding.Unicode.GetBytes(text);

        private void SendData(byte[] data)
            => UseNetworkStream(netStream => netStream.Write(data, 0, data.Length));

        private void SendData(Stream readerStream)
            => UseNetworkStream(netStream => readerStream.CopyTo(netStream, _bufferSize));

        private async Task SendDataAsync(byte[] data, CancellationToken cancellationToken)
            => await UseNetworkStreamAsync(async netStream
                                           => await netStream.WriteAsync(data, 0, data.Length, cancellationToken));

        private async Task SendDataAsync(Stream readerStream, CancellationToken cancellationToken)
            => await UseNetworkStreamAsync(async netStream
                                           => await readerStream.CopyToAsync(netStream, _bufferSize, cancellationToken));

        private static string Serialize<T>(T obj)
            => new JsonModelSerializer().Serialize(obj);

        private void UseNetworkStream(Action<Stream> useStreamAction)
        {
            using (var tcpClient = CreateTcpClient())
            {
                using (var netStream = tcpClient.GetStream())
                {
                    useStreamAction(netStream);
                }
            }
        }

        private async Task UseNetworkStreamAsync(Func<Stream, Task> useStreamAction)
        {
            using (var tcpClient = await CreateTcpClientAsync())
            {
                using (var netStream = tcpClient.GetStream())
                {
                    await useStreamAction(netStream);
                }
            }
        }
        #endregion
    }
}