namespace CB.Net.Socket
{
    public class TcpSocketConfiguration: ConfigurationBase<TcpSocketConfigurationSection>
    {
        #region  Constructors & Destructor
        public TcpSocketConfiguration(string configSectionName): base(configSectionName) { }

        public TcpSocketConfiguration(): this("tcpSocketConfig") { }
        #endregion
    }

    /*public class TcpSocketConfiguration
    {
        #region  Properties & Indexers
        public int Backlog { get; set; } = GetBacklog();
        public int BufferSize { get; set; } = GetBufferSize();
        public string IpAddress { get; set; } = GetIpAddress();
        public int Port { get; set; } = GetPort();
        #endregion


        #region Methods
        public static int GetBacklog()
            => GetSettingInt("backlog", TcpSocketParams.BACKLOG);

        public static int GetBufferSize()
            => GetSettingInt("bufferSize", TcpSocketParams.BUFFER_SIZE);

        public static string GetIpAddress()
            => GetSettingString("ipAddress", TcpSocketParams.IP_ADDRESS);

        public static int GetPort()
            => GetSettingInt("port", TcpSocketParams.PORT);
        #endregion


        #region Implementation
        private static string GetSetting(string setting)
            => ConfigurationManager.AppSettings[setting];

        private static int GetSettingInt(string setting, int defaultValue)
        {
            string s;
            int i;
            return (s = GetSetting(setting)) == null || !int.TryParse(s, out i) ? defaultValue : i;
        }

        private static string GetSettingString(string setting, string defaultValue)
            => GetSetting(setting) ?? defaultValue;
        #endregion
    }*/
}