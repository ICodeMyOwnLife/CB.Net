namespace CB.Net.Socket
{
    internal class TcpSocketParams
    {
        #region Fields
        internal const int BACKLOG = 100;
        public const int BUFFER_SIZE = 1 << 15;
        internal const string IP_ADDRESS = "127.0.0.1";
        internal const int PORT = 11000;
        #endregion
    }
}