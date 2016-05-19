using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CB.Model.Serialization;


namespace CB.Net.Socket
{
    public class TcpSocketServer
    {
        public delegate string ProvideFilePathCallback(string fileName);


        #region Fields
        private const double MINIMUM_PROGRESS_INTERVAL = 0.006172;
        private readonly int _backlog;
        private readonly string _ipAddress;
        private readonly int _port;
        private TcpListener _tcpListener;
        #endregion


        #region  Constructors & Destructor
        public TcpSocketServer(string ipAddress = TcpSocketParams.IP_ADDRESS, int port = TcpSocketParams.PORT,
            int backlog = TcpSocketParams.BACKLOG)
        {
            _ipAddress = ipAddress;
            _port = port;
            _backlog = backlog;
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

        public async Task ReceiveFileAsync(ProvideFilePathCallback provideFilePathCallback,
            CancellationToken cancellationToken, IProgress<double> progressReporter = null)
        {
            var fileInfo = await ReceiveObjectAsync<NetFileInfo>(cancellationToken);
            var filePath = provideFilePathCallback(fileInfo.FileName);

            using (var writer = File.OpenWrite(filePath))
            {
                double totalBytesRead = 0, progress = 0;
                progressReporter?.Report(0);

                await ReceiveData((buffer, bytesRead) =>
                {
                    writer.Write(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                    var newProgress = totalBytesRead / fileInfo.FileSize;
                    if (!(newProgress - progress > MINIMUM_PROGRESS_INTERVAL)) return;

                    progress = newProgress;
                    progressReporter?.Report(progress);
                }, cancellationToken);
                progressReporter?.Report(1);
            }
        }

        public async Task<T> ReceiveObjectAsync<T>(CancellationToken cancellationToken)
            => Deserialize<T>(await ReceiveTextAsync(cancellationToken));

        public async Task<string> ReceiveTextAsync(CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();
            await ReceiveData((buffer, bytesRead) =>
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

        private static string GetDataText(byte[] buffer, int bytesRead)
            => Encoding.Unicode.GetString(buffer, 0, bytesRead);

        private async Task ReceiveData(Action<byte[], int> onReceiveData, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024]; // TODO: bufferSize

            using (var client = await _tcpListener.AcceptTcpClientAsync())
            {
                using (var netStream = client.GetStream())
                {
                    int bytesRead;
                    while ((bytesRead = await netStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
                    {
                        onReceiveData(buffer, bytesRead);
                    }
                }
            }
        }
        #endregion
    }
}