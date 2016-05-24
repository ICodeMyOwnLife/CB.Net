using System.Configuration;


namespace CB.Net.SignalR
{
    public class SignalRConfigSection: ConfigurationSection
    {
        #region Fields
        protected const string HUB_NAME_ATTR = "hubName";
        protected const string URL_ATTR = "url";
        #endregion


        #region  Properties & Indexers
        [ConfigurationProperty(HUB_NAME_ATTR, DefaultValue = SignalRDefaults.HUB_NAME, IsRequired = false)]
        public virtual string HubName
        {
            get { return (string)this[HUB_NAME_ATTR]; }
            set { this[HUB_NAME_ATTR] = value; }
        }

        [ConfigurationProperty(URL_ATTR, DefaultValue = SignalRDefaults.URL, IsRequired = false)]
        public virtual string Url
        {
            get { return (string)this[URL_ATTR]; }
            set { this[URL_ATTR] = value; }
        }
        #endregion
    }
}