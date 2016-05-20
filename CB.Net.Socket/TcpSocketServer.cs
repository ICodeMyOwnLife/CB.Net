using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CB.Model.Common;
using CB.Model.Serialization;


namespace CB.Net.Socket
{
    public class TcpSocketServer
    {
        #region Fields
        private readonly int _backlog;
        private readonly int _bufferSize;
        private readonly string _ipAddress;
        private readonly int _port;
        private TcpListener _tcpListener;
        #endregion


        #region  Constructors & Destructor
        public TcpSocketServer(TcpSocketConfiguration configuration)
            : this(configuration.ConfigurationSection.IpAddress, configuration.ConfigurationSection.Port,
                configuration.ConfigurationSection.Backlog, configuration.ConfigurationSection.BufferSize) { }

        public TcpSocketServer(string ipAddress = TcpSocketParams.IP_ADDRESS, int port = TcpSocketParams.PORT,
            int backlog = TcpSocketParams.BACKLOG, int bufferSize = TcpSocketParams.BUFFER_SIZE)
        {
            _ipAddress = ipAddress;
            _port = port;
            _backlog = backlog;
            _bufferSize = bufferSize;
        }
        #endregion


        #region Methods
        public void Connect()
        {
            _tcpListener = new TcpListener(IPAddress.Parse(_ipAddress), _port);
            _tcpListener.Start(_backlog);
        }

        public void Disconnect()
        {
            _tcpListener.Stop();
        }

        public void ReceiveFile(ProvideFilePathCallback provideFilePathCallback)
        {
            var fileInfo = ReceiveObject<NetFileInfo>();
            var filePath = provideFilePathCallback(fileInfo.FileName);

            UseNetworkStream(netStream =>
            {
                using (var writer = OpenWriterStream(filePath))
                {
                    netStream.CopyTo(writer, _bufferSize);
                }
            });
        }

        public async Task ReceiveFileAsync(ProvideFilePathCallback provideFilePathCallback,
            IProgress<long> progressReporter)
            => await ReceiveFileAsync(provideFilePathCallback, CancellationToken.None, progressReporter);

        public async Task ReceiveFileAsync(ProvideFilePathCallback provideFilePathCallback,
            CancellationToken cancellationToken, IProgress<long> progressReporter)
        {
            var fileInfo = await ReceiveObjectAsync<NetFileInfo>(cancellationToken);
            var filePath = provideFilePathCallback(fileInfo.FileName);
            var fileProgressReporter = progressReporter as IReportFileProgress;
            if (fileProgressReporter != null) fileProgressReporter.FileSize = fileInfo.FileSize;

            using (var writer = OpenWriterStream(filePath))
            {
                long totalBytesRead = 0;
                progressReporter?.Report(0);

                await ReceiveDataAsync(async (buffer, bytesRead) =>
                {
                    await writer.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    totalBytesRead += bytesRead;
                    progressReporter?.Report(totalBytesRead);
                }, cancellationToken);
            }
        }

        public async Task ReceiveFileAsync(ProvideFilePathCallback provideFilePathCallback)
            => await ReceiveFileAsync(provideFilePathCallback, CancellationToken.None);

        public async Task ReceiveFileAsync(ProvideFilePathCallback provideFilePathCallback,
            CancellationToken cancellationToken)
        {
            var fileInfo = await ReceiveObjectAsync<NetFileInfo>(cancellationToken);
            var filePath = provideFilePathCallback(fileInfo.FileName);

            await UseNetworkStreamAsync(async netStream =>
            {
                using (var writer = OpenWriterStream(filePath))
                {
                    await netStream.CopyToAsync(writer, _bufferSize, cancellationToken);
                }
            });
        }

        public T ReceiveObject<T>()
            => Deserialize<T>(ReceiveText());

        public async Task<T> ReceiveObjectAsync<T>(CancellationToken cancellationToken)
            => Deserialize<T>(await ReceiveTextAsync(cancellationToken));

        public string ReceiveText()
        {
            var sb = new StringBuilder();
            ReceiveData((buffer, bytesRead) =>
            {
                var text = GetDataText(buffer, bytesRead);
                sb.Append(text);
            });
            return sb.ToString();
        }

        public async Task<string> ReceiveTextAsync(CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();
            await ReceiveDataAsync((buffer, bytesRead) =>
            {
                var text = GetDataText(buffer, bytesRead);
                sb.Append(text);
            }, cancellationToken);
            return sb.ToString();
        }
        #endregion


        #region Implementation
        private static T Deserialize<T>(string contents)
            => new JsonModelSerializer().Deserialize<T>(contents);

        private TResult FetchNetworkStream<TResult>(Func<Stream, TResult> fetchNetworkStreamAction)
        {
            using (var client = _tcpListener.AcceptTcpClient())
            {
                using (var netStream = client.GetStream())
                {
                    return fetchNetworkStreamAction(netStream);
                }
            }
        }

        private async Task<TResult> FetchNetworkStreamAsync<TResult>(
            Func<Stream, Task<TResult>> fetchNetworkStreamAction)
        {
            using (var client = await _tcpListener.AcceptTcpClientAsync())
            {
                using (var netStream = client.GetStream())
                {
                    return await fetchNetworkStreamAction(netStream);
                }
            }
        }

        private static string GetDataText(byte[] buffer, int bytesRead)
            => Encoding.Unicode.GetString(buffer, 0, bytesRead);

        private static FileStream OpenWriterStream(string filePath)
            => File.OpenWrite(filePath);

        private void ReceiveData(Action<byte[], int> onReceiveData)
            => UseNetworkStream(netStream =>
            {
                var buffer = new byte[_bufferSize];
                int bytesRead;
                while ((bytesRead = netStream.Read(buffer, 0, _bufferSize)) != 0)
                {
                    onReceiveData(buffer, bytesRead);
                }
            });

        private async Task ReceiveDataAsync(Action<byte[], int> onReceiveData, CancellationToken cancellationToken)
            => await UseNetworkStreamAsync(async netStream =>
            {
                var buffer = new byte[_bufferSize];
                int bytesRead;
                while ((bytesRead = await netStream.ReadAsync(buffer, 0, _bufferSize, cancellationToken)) != 0)
                {
                    onReceiveData(buffer, bytesRead);
                }
            });

        private async Task ReceiveDataAsync(Func<byte[], int, Task> onReceiveData, CancellationToken cancellationToken)
            => await UseNetworkStreamAsync(async netStream =>
            {
                var buffer = new byte[_bufferSize];
                int bytesRead;
                while ((bytesRead = await netStream.ReadAsync(buffer, 0, _bufferSize, cancellationToken)) != 0)
                {
                    await onReceiveData(buffer, bytesRead);
                }
            });

        private void UseNetworkStream(Action<Stream> useNetworkStreamAction)
        {
            using (var client = _tcpListener.AcceptTcpClient())
            {
                using (var netStream = client.GetStream())
                {
                    useNetworkStreamAction(netStream);
                }
            }
        }

        private async Task UseNetworkStreamAsync(Func<Stream, Task> useNetworkStreamAction)
        {
            using (var client = await _tcpListener.AcceptTcpClientAsync())
            {
                using (var netStream = client.GetStream())
                {
                    await useNetworkStreamAction(netStream);
                }
            }
        }
        #endregion
    }
}