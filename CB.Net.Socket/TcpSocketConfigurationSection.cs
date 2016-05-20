using System.Configuration;


namespace CB.Net.Socket
{
    public class TcpSocketConfigurationSection: ConfigurationSection
    {
        #region Fields
        private const string BACKLOG_ATTR = "backlog";
        private const string BUFFER_SIZE_ATTR = "bufferSize";
        private const string IP_ADDRESS_ATTR = "ipAddress";
        private const string PORT_ATTR = "port";
        #endregion


        #region  Properties & Indexers
        [ConfigurationProperty(BACKLOG_ATTR, DefaultValue = TcpSocketParams.BACKLOG, IsRequired = false)]
        public int Backlog
        {
            get { return (int)this[BACKLOG_ATTR]; }
            set { this[BACKLOG_ATTR] = value; }
        }

        [ConfigurationProperty(BUFFER_SIZE_ATTR, DefaultValue = TcpSocketParams.BUFFER_SIZE, IsRequired = false)]
        public int BufferSize
        {
            get { return (int)this[BUFFER_SIZE_ATTR]; }
            set { this[BUFFER_SIZE_ATTR] = value; }
        }

        [ConfigurationProperty(IP_ADDRESS_ATTR, DefaultValue = TcpSocketParams.IP_ADDRESS, IsRequired = false)]
        public string IpAddress
        {
            get { return (string)this[IP_ADDRESS_ATTR]; }
            set { this[IP_ADDRESS_ATTR] = value; }
        }

        [ConfigurationProperty(PORT_ATTR, DefaultValue = TcpSocketParams.PORT, IsRequired = false)]
        public int Port
        {
            get { return (int)this[PORT_ATTR]; }
            set { this[PORT_ATTR] = value; }
        }
        #endregion
    }
}


// TODO: Move ConfigurationBase to another assembly